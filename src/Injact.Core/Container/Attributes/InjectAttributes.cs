namespace Injact.Core.Container.Attributes;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class InjectAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
public class InjectOptionalAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Parameter)]
public class InjectIgnoreAttribute : Attribute { }