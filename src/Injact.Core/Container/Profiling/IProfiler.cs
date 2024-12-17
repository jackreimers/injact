namespace Injact.Core.Container.Profiling;

public interface IProfiler
{
    public Profile Start(string message);

    public Profile Start(string message, bool condition);
}