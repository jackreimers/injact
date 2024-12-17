namespace Injact.Core.Container.Binding;

public class FactoryBinding : IBinding
{
    public Type InterfaceType { get; }
    public Type ConcreteType { get; }
    public Type ObjectType { get; }

    public List<Type> AllowedInjectionTypes { get; } = new();

    public FactoryBinding(Type interfaceType, Type concreteType, Type objectType)
    {
        InterfaceType = interfaceType;
        ConcreteType = concreteType;
        ObjectType = objectType;
    }
}