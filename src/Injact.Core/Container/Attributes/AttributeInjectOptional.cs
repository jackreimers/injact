namespace Injact.Core.Container.Attributes;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class InjectOptionalAttribute : Attribute { }