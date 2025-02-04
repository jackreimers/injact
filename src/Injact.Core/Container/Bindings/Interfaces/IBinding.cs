namespace Injact.Core.Container.Bindings.Interfaces;

public interface IBinding
{
    public Type InterfaceType { get; }
    public Type ConcreteType { get; }

    public List<Type> AllowedInjectionTypes { get; }
}