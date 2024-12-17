namespace Injact.Godot;

public class Logger<T> : ILogger
{
    private readonly ContainerOptions _options;
    private readonly string _typeName;

    public Logger(ContainerOptions options)
    {
        _options = options;

        var typeName = typeof(T).Name;
        _typeName = typeName is nameof(DiContainer) or nameof(Context)
            ? "Injact"
            : typeName;
    }

    public void LogInformation(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Information)
        {
            GD.Print($"[{_typeName}] {message}");
        }
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Warning)
        {
            GD.PushWarning($"[{_typeName}] {message}");
        }
    }

    public void LogError(string message, bool condition = true)
    {
        if (condition && _options.LoggingLevel >= LoggingLevel.Error)
        {
            GD.PushError($"[{_typeName}] {message}");
        }
    }

    public void LogTrace(string message, object[] arguments)
    {
        GD.Print($"[Trace] {string.Format(message, arguments)}");
    }
}