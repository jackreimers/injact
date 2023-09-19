using System.Collections.Generic;
using System.Linq;
using Godot;
using Injact.Internal;

namespace Injact;

public partial class Context : Node
{
    [ExportCategory("Initialisation")]
    [Export] private bool searchForNodes = true;
    [Export] private bool searchForInstallers;
    
    [ExportCategory("References")]
    [Export] private Node[] nodes;
    [Export] private NodeInstaller[] installers;
    
    //TODO: Investigate better way to set flag options for export
    [ExportCategory("Logging")]
    [Export(PropertyHint.Flags, "Information")] private int loggingLevels = 0;
    [Export(PropertyHint.Flags, "Startup,Resolution")] private int profilingLevels = 0;

    private DiContainer _container;
    private Injector _injector;

    private LoggingFlags loggingFlags;
    private ProfilingFlags profilingFlags;

    public override void _EnterTree()
    {
        loggingFlags = (LoggingFlags)loggingLevels;
        profilingFlags = (ProfilingFlags)profilingLevels;
        
        var profile = GodotHelpers.ProfileIf(profilingFlags.HasFlag(ProfilingFlags.Startup), "Initialised dependency injection in {0}ms.");

        _container = new DiContainer(loggingFlags, profilingFlags);
        _injector = _container.Resolve<Injector>(this);

        List<Node> nodes = null!;

        if (searchForInstallers)
        {
            var installerProfile = GodotHelpers.ProfileIf(profilingFlags.HasFlag(ProfilingFlags.Startup), "Found {1} installers in {0}ms.");

            GodotHelpers.WarnIf(installers.Any(), "Search for installers is enabled, user set installers will be ignored.");
            nodes = GodotHelpers.GetAllChildNodes(GetTree().Root);

            installers = nodes
                .Where(s => s is NodeInstaller)
                .Cast<NodeInstaller>()
                .ToArray();

            GodotHelpers.WarnIf(!installers.Any(), "Could not find any node installers in scene.");

            installerProfile?.Invoke(new object[] { installers.Length });
        }

        foreach (var installer in installers)
        {
            _injector.InjectInto(installer);
            installer.InstallBindings();
        }

        _container.ProcessPendingBindings();

        if (searchForNodes)
            ResolveAllInScene(nodes);

        profile?.Invoke(null);
        base._EnterTree();
    }

    private void ResolveAllInScene(List<Node> nodes)
    {
        var profile = GodotHelpers.ProfileIf(profilingFlags.HasFlag(ProfilingFlags.Startup), "Found {1} nodes in {0}ms.");
        nodes ??= GodotHelpers.GetAllChildNodes(GetTree().Root);

        foreach (var node in nodes)
            _injector.InjectInto(node);

        profile?.Invoke(new object[] { nodes.Count });
    }
}