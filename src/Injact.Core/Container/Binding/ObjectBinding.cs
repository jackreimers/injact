namespace Injact.Core.Container.Binding;

public class ObjectBinding : IBinding
{
    private const string BindingLockedSingletonMessage = "Cannot change singleton setting after binding has been locked.";

    private bool isSingleton;
    private object? instance;

    public bool IsLocked { get; private set; }

    public bool IsSingleton
    {
        get => isSingleton;
        set => OnSingletonChanged(value);
    }

    public bool IsImmediate { get; set; }
    public Type InterfaceType { get; }
    public Type ConcreteType { get; }
    public Type[] ImplementedInterfaces { get; }
    public List<Type> AllowedInjectionTypes { get; }

    public object? Instance
    {
        get => instance;
        set => OnInstanceChanged(value);
    }

    public ObjectBinding(Type interfaceType, Type concreteType)
    {
        InterfaceType = interfaceType;
        ConcreteType = concreteType;
        Instance = null;
        ImplementedInterfaces = concreteType.GetInterfaces();
        AllowedInjectionTypes = new List<Type>();
    }

    public ObjectBinding(Type interfaceType, Type concreteType, object instance)
    {
        InterfaceType = interfaceType;
        ConcreteType = concreteType;
        Instance = instance;
        ImplementedInterfaces = concreteType.GetInterfaces();
        AllowedInjectionTypes = new List<Type>();
    }

    public void Lock()
    {
        IsLocked = true;
    }

    private void OnSingletonChanged(bool value)
    {
        if (value == isSingleton)
        {
            return;
        }

        Guard.Against.Condition(IsLocked, BindingLockedSingletonMessage);
        isSingleton = value;

        if (instance != null)
        {
            Lock();
        }
    }

    private void OnInstanceChanged(object? value)
    {
        if (value == Instance)
        {
            return;
        }

        Guard.Against.Condition(IsLocked, BindingLockedSingletonMessage);
        isSingleton = true;
        instance = value;

        Lock();
    }
}