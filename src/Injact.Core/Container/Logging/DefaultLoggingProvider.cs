namespace Injact.Core.Container.Logging;

public class DefaultLoggingProvider : ILoggingProvider
{
    public ILogger GetLogger<T>(ContainerOptions options)
    {
        return new DefaultLogger<T>(options);
    }

    public Type GetLoggerType()
    {
        return typeof(DefaultLogger<>);
    }
}