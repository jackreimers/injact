namespace Injact.Core.Container.Logging;

public class DefaultLogger<T> : ILogger
{
    private readonly ContainerOptions _options;
    private readonly string _typeName;

    public DefaultLogger(ContainerOptions options)
    {
        _options = options;

        var typeName = typeof(T).Name;
        _typeName = typeName is nameof(DiContainer)
            ? "Injact"
            : typeName;

        LogWarning("DefaultLogger is being used, no messages will be logged to the game engine console!");
    }

    public void LogInformation(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Information)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Warning)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogError(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Error)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogTrace(string message, object[] arguments)
    {
        Console.WriteLine($"[Trace] {string.Format(message, arguments)}");
    }
}