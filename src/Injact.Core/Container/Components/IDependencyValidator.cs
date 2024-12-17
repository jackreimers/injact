namespace Injact.Core.Container.Components;

internal interface IDependencyValidator
{
    public bool CanCreate(Type type, bool throwOnNotFound);

    public void CheckForIllegalInjection(IBinding binding, Type? requestingType);

    public void CheckForCircularInjection(Type requestedType, object[] arguments);
}