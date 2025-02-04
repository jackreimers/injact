namespace Injact.Core.Container.Bindings;

public class FactoryBinding : IBinding
{
    public FactoryBinding(
        Type interfaceType,
        Type concreteType,
        Type objectType)
    {
        InterfaceType = interfaceType;
        ConcreteType = concreteType;
        ObjectType = objectType;
    }

    public Type ObjectType { get; }

    public Type InterfaceType { get; }
    public Type ConcreteType { get; }

    public List<Type> AllowedInjectionTypes { get; } = new();
}