namespace Injact.Core.Container.Logging.Interfaces;

public interface ILoggingProvider
{
    public ILogger GetLogger<T>(ContainerOptions options);

    public Type GetLoggerType();
}