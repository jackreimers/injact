using Injact.Core.Container;
using Injact.Core.Container.Components;
using Injact.Core.Container.Exceptions;

namespace Injact.Tests;

public class TestsBindingObjects
{
    [Fact]
    public void Bind_DefaultBindings_Resolves()
    {
        var container = new DiContainer();

        var resolvedContainer = container.Resolve<IDiContainer>(typeof(Consumer));
        var resolvedInjector = container.Resolve<IDependencyInjector>(typeof(Consumer));

        Assert.NotNull(resolvedContainer);
        Assert.NotNull(resolvedInjector);
    }

    [Fact]
    public void Bind_InterfaceToContainer_ReturnsInstanceForInterface()
    {
        var container = new DiContainer();
        container.Bind<Interface1, Class1>();

        var resolved = container.Resolve<Interface1>(typeof(Consumer));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_ConcreteToContainer_ReturnsInstanceForConcrete()
    {
        var container = new DiContainer();
        container.Bind<Class1>();

        var resolved = container.Resolve<Class1>(typeof(Consumer));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsSameInstance()
    {
        var container = new DiContainer();
        container.Bind<Class1>().AsSingleton();

        var resolved1 = container.Resolve<Class1>(typeof(Consumer));
        var resolved2 = container.Resolve<Class1>(typeof(Consumer));

        Assert.Same(resolved1, resolved2);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsExistingInstance()
    {
        var instance = new Class1();
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .FromInstance(instance)
            .AsSingleton();

        var resolved = container.Resolve<Class1>(typeof(Consumer));

        Assert.Same(instance, resolved);
    }

    [Fact]
    public void Bind_NonSingletonToContainer_ReturnsDifferentInstances()
    {
        var container = new DiContainer();
        container.Bind<Class1>();

        var resolved1 = container.Resolve<Class1>(typeof(Consumer));
        var resolved2 = container.Resolve<Class1>(typeof(Consumer));

        Assert.NotSame(resolved1, resolved2);
    }

    [Fact]
    public void Bind_ExistingBinding_ThrowsException()
    {
        var container = new DiContainer();
        container.Bind<Class1>();

        Assert.Throws<DependencyException>(() => container.Bind<Class1>());
    }

    [Fact]
    public void Bind_CircularDependency_ThrowsException()
    {
        var container = new DiContainer();

        container.Bind<CircularDependency1>();
        container.Bind<CircularDependency2>();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<CircularDependency1>(typeof(Consumer)));
    }

    [Fact]
    public void Bind_CircularDependencyNested_ThrowsException()
    {
        var container = new DiContainer();

        container.Bind<CircularDependencyNested1>();
        container.Bind<CircularDependencyNested2>();
        container.Bind<CircularDependencyNested3>();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<CircularDependencyNested1>(typeof(Consumer)));
    }
}