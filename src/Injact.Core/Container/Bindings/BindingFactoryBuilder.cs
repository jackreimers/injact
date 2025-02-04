namespace Injact.Core.Container.Bindings;

public class FactoryBindingBuilder
{
    private readonly FactoryBinding _binding;

    public FactoryBindingBuilder(FactoryBinding binding)
    {
        _binding = binding;
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
}