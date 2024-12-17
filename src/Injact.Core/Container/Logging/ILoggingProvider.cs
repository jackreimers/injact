namespace Injact.Core.Container.Logging;

public interface ILoggingProvider
{
    public ILogger GetLogger<T>(ContainerOptions options);

    public Type GetLoggerType();
}