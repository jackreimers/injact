namespace Injact.Core.Container.Exceptions;

public class OptionsExeption : Exception
{
    public OptionsExeption() { }

    public OptionsExeption(string message)
        : base(message) { }

    public OptionsExeption(string message, Exception inner)
        : base(message, inner) { }
}