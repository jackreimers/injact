namespace Injact.Core.Container.Binding;

public class FactoryBindingBuilder
{
    private readonly FactoryBinding _binding;

    public FactoryBindingBuilder(FactoryBinding binding)
    {
        _binding = binding;
    }

    [Obsolete("WithType is redundant and will be removed in the future.")]
    public FactoryBindingBuilder WithType<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        return this;
    }

    public FactoryBindingBuilder WhenInjectedInto<TValue>()
    {
        _binding.AllowedInjectionTypes.Add(typeof(TValue));
        return this;
    }

    public FactoryBindingBuilder WhenInjectedInto(Type allowedType)
    {
        _binding.AllowedInjectionTypes.Add(allowedType);
        return this;
    }

    [Obsolete("Calling finalise is no longer required.")]
    public void Finalise() { }
}