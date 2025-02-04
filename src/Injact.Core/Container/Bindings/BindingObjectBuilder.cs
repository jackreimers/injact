namespace Injact.Core.Container.Bindings;

public class ObjectBindingBuilder
{
    private readonly ObjectBinding _binding;

    public ObjectBindingBuilder(ObjectBinding binding)
    {
        _binding = binding;
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

    public ObjectBindingBuilder Immediate()
    {
        _binding.IsImmediate = true;
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
}