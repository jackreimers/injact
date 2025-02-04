namespace Injact.Tests;

public class TestsInjection
{
    [Fact]
    public void Inject_ConstructorInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<IInterface, Class1>();
        container.Bind<Class2>();
        container.Bind<ConstructorInjection>();

        var resolved = container.Resolve<ConstructorInjection>(typeof(Consumer));

        Assert.NotNull(resolved);
        Assert.NotNull(resolved.TestClass);
        Assert.NotNull(resolved.TestInterface);
    }

    [Fact]
    public void Inject_PublicReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionFieldReadonlyPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionFieldReadonlyProtected();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionFieldReadonlyPrivate();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PropertyInjection_Succeeds()
    {
        //Note: Property injection works by setting the value of the backing field
        //If it works on one property it will work on all of them regardless of access modifier or if there is a setter
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionPropertyPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PublicMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionMethodPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionMethodProtected();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<Class2>();
        container.Bind<IInterface, Class1>();

        var instance = new InjectionMethodPrivate();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_IllegalInjection_ThrowsException()
    {
        var container = new DiContainer();

        container.Bind<IInterface, Class1>().WhenInjectedInto<Class2>();
        container.Bind<ConstructorInjection>();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<ConstructorInjection>(typeof(Consumer)));
    }
}