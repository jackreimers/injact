namespace Injact.Core.Container.Components.Interfaces;

public interface IInjector
{
    public void InjectInto(params object[] targets);
}