namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyPrivate
{
    [field: Inject] public Class2 TestClass { get; } = null!;
    [field: Inject] public IInterface TestInterface { get; } = null!;
}