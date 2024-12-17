using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyPublic
{
    [Inject] public readonly Class2 TestClass = null!;
    [Inject] public readonly Interface1 TestInterface = null!;
}
