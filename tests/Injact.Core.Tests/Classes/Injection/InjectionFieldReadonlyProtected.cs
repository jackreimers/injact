//ReSharper disable MemberCanBePrivate.Global

namespace Injact.Tests.Classes;

public class InjectionFieldReadonlyProtected
{
    [Inject] protected readonly Class2 testClass = null!;
    [Inject] protected readonly IInterface testInterface = null!;

    public Class2 TestClass => testClass;
    public IInterface TestInterface => testInterface;
}