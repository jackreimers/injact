namespace Injact.Core.Container.Factories;

public interface IFactory<TInterface> : IFactory
{
    public TInterface Create(bool deferInitialisation = false);

    public TInterface Create(params object[] arguments);

    public TInterface Create(bool deferInitialisation, params object[] arguments);

    public TInterface Create<TConcrete>(bool deferInitialisation = false)
        where TConcrete : class, TInterface;

    public TInterface Create<TConcrete>(params object[] arguments)
        where TConcrete : class, TInterface;

    public TInterface Create<TConcrete>(bool deferInitialisation, params object[] arguments)
        where TConcrete : class, TInterface;
}

public interface IFactory { }