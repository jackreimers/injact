namespace Injact.Core.Container.Options;

public class Options<T> : IOptions<T>
{

    public Options(T value)
    {
        Value = value;
    }

    public T Value { get; }
}