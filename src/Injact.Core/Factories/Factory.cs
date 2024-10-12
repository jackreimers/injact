namespace Injact.Factories;

public sealed class Factory<TInterface> : IFactory<TInterface>
{
    private readonly DiContainer _container;

    public Factory(DiContainer container)
    {
        _container = container;
    }

    public TInterface Create(bool deferInitialisation = false)
    {
        return (TInterface)_container.Create(typeof(TInterface), deferInitialisation, true, Array.Empty<object>());
    }

    public TInterface Create(params object[] arguments)
    {
        return (TInterface)_container.Create(typeof(TInterface), false, true, arguments);
    }

    public TInterface Create(bool deferInitialisation, params object[] arguments)
    {
        return (TInterface)_container.Create(typeof(TInterface), deferInitialisation, true, arguments);
    }

    public TInterface Create<TConcrete>(bool deferInitialisation = false)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), deferInitialisation, true, Array.Empty<object>());
    }

    public TInterface Create<TConcrete>(params object[] arguments)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), false, true, arguments);
    }

    public TInterface Create<TConcrete>(bool deferInitialisation, params object[] arguments)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), deferInitialisation, true, arguments);
    }
}