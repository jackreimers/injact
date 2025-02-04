namespace Injact.Tests.Classes;

public class ConstructorInjection
{
    public ConstructorInjection(IInterface testInterface, Class2 testClass)
    {
        TestInterface = testInterface;
        TestClass = testClass;
    }

    public IInterface TestInterface { get; }
    public Class2 TestClass { get; }
}