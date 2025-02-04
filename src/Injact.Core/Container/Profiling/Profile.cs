namespace Injact.Core.Container.Profiling;

public class Profile : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _message;
    private readonly bool _condition;
    private readonly Stopwatch _stopwatch = new();

    public Profile(
        ILogger logger,
        string message,
        bool condition = true)
    {
        _logger = logger;
        _message = message;
        _condition = condition;

        _stopwatch.Start();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Stop();
    }

    private void Stop()
    {
        _stopwatch.Stop();

        if (_condition)
        {
            _logger.LogTrace(
                _message,
                new object[] { _stopwatch.ElapsedMilliseconds + "ms" });
        }
    }
}