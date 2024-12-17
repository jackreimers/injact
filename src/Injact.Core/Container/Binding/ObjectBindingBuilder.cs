namespace Injact.Core.Container.Binding;

public class ObjectBindingBuilder
{
    private readonly ObjectBinding _binding;

    public ObjectBindingBuilder(ObjectBinding binding)
    {
        _binding = binding;
    }

    [Obsolete("WithType is redundant and will be removed in the future.")]
    public ObjectBindingBuilder WithType<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        return this;
    }

    public ObjectBindingBuilder FromInstance(object value)
    {
        _binding.Instance = value;
        return this;
    }

    public ObjectBindingBuilder AsSingleton()
    {
        _binding.IsSingleton = true;
        return this;
    }

    [Obsolete("AsTransient is redundant and will be removed in the future.")]
    public ObjectBindingBuilder AsTransient()
    {
        return this;
    }

    public ObjectBindingBuilder Immediate()
    {
        _binding.IsImmediate = true;
        return this;
    }

    [Obsolete("Delayed is redundant and will be removed in the future.")]
    public ObjectBindingBuilder Delayed()
    {
        return this;
    }

    public ObjectBindingBuilder WhenInjectedInto<T>()
    {
        _binding.AllowedInjectionTypes.Add(typeof(T));
        return this;
    }

    public ObjectBindingBuilder WhenInjectedInto(params Type[] value)
    {
        _binding.AllowedInjectionTypes.AddRange(value);
        return this;
    }

    [Obsolete("Calling finalise is no longer required.")]
    public void Finalise() { }
}