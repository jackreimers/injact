namespace Injact.Core.Container.Profiling;

public class Profiler : IProfiler
{
    private readonly ILogger _logger;

    public Profiler(ILogger logger)
    {
        _logger = logger;
    }

    public Profile Start(string message)
    {
        return new Profile(_logger, message);
    }

    public Profile Start(string message, bool condition)
    {
        return new Profile(_logger, message, condition);
    }
}