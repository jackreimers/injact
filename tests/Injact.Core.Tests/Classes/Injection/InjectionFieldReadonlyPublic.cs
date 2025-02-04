namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyPublic
{
    [Inject] public readonly Class2 TestClass = null!;
    [Inject] public readonly IInterface TestInterface = null!;
}