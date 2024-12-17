namespace Injact.Core.Container.Components;

public interface IDependencyInjector
{
    public void InjectInto(params object[] targets);
}