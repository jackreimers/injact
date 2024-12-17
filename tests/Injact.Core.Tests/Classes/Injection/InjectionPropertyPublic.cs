using Injact.Core.Container.Attributes;

namespace Injact.Tests.Classes;

public class InjectionPropertyPublic
{
    [Inject] public Class2 TestClass { get; set; } = null!;
    [Inject] public Interface1 TestInterface { get; set; } = null!;
}
