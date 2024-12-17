namespace Injact.Godot;

public class LoggingProvider : ILoggingProvider
{
    public ILogger GetLogger<T>(ContainerOptions options)
    {
        return new Logger<T>(options);
    }

    public Type GetLoggerType()
    {
        return typeof(Logger<>);
    }
}