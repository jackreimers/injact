using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyProtected
{
    [Inject] protected readonly Class2 testClass = null!;
    [Inject] protected readonly Interface1 testInterface = null!;

    public Class2 TestClass => testClass;
    public Interface1 TestInterface => testInterface;
}
