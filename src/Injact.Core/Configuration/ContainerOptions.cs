﻿namespace Injact;

public class ContainerOptions
{
    public bool LogDebugging { get; set; }
    public bool LogTracing { get; set; }
    public bool UseAutoFactories { get; set; } = true;
    public bool InjectIntoDefaultProperties { get; set; }

    public ILoggingProvider LoggingProvider { get; init; } = null!;
}