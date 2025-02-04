namespace Injact.Core.Container.Exceptions;

public class DependencyException : Exception
{
    public DependencyException() { }

    public DependencyException(string message)
        : base(message) { }

    public DependencyException(string message, Exception inner)
        : base(message, inner) { }
}