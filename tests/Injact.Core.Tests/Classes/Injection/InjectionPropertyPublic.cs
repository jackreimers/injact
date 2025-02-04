namespace Injact.Tests.Classes;

public class InjectionPropertyPublic
{
    [Inject] public Class2 TestClass { get; set; } = null!;
    [Inject] public IInterface TestInterface { get; set; } = null!;
}