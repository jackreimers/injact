using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionMethodPublic
{
    public Class2 TestClass { get; private set; } = null!;
    public Interface1 TestInterface { get; private set; } = null!;

    [Inject]
    public void Inject(Class2 testClass, Interface1 testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}
