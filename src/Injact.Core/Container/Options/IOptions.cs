namespace Injact.Core.Container.Options;

public interface IOptions<out T>
{
    public T Value { get; }
}