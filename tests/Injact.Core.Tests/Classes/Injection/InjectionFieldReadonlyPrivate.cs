using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyPrivate
{
    [field: Inject] public Class2 TestClass { get; } = null!;
    [field: Inject] public Interface1 TestInterface { get; } = null!;
}
