namespace Injact.Tests.Classes;

public class InjectionMethodProtected
{
    public Class2 TestClass { get; private set; } = null!;
    public IInterface TestInterface { get; private set; } = null!;

    [Inject]
    protected void Inject(Class2 testClass, IInterface testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}