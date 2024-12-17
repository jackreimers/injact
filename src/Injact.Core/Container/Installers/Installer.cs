namespace Injact.Core.Container.Installers;

public abstract class Installer : IInstaller
{
    protected readonly DiContainer _container;

    protected Installer(DiContainer container)
    {
        _container = container;
    }

    public abstract void InstallBindings();
}