namespace Injact.Core.Container.Options.Interfaces;

public interface IOptions<out T>
{
    public T Value { get; }
}