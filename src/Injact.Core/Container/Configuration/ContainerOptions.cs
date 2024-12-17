namespace Injact.Core.Container.Configuration;

public class ContainerOptions
{
    public LoggingLevel LoggingLevel { get; init; }

    public bool LogTracing { get; init; }
    public bool UseAutoFactories { get; init; } = true;
    public bool InjectIntoDefaultProperties { get; init; }

    public ILoggingProvider LoggingProvider { get; init; } = null!;
}