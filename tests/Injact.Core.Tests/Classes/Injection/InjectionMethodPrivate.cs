using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionMethodPrivate
{
    public Class2 TestClass { get; private set; } = null!;
    public Interface1 TestInterface { get; private set; } = null!;

    [Inject]
    private void Inject(Class2 testClass, Interface1 testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}
