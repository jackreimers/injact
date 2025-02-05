namespace Injact.Core.Container.Components;

internal class Validator
{
    private const string IllegalInjectionMessage = "\"{0}\" requested type of \"{1}\" when it is not allowed to.";
    private const string CircularDependencyMessage = "Requested type of \"{0}\" contains a circular dependency.";
    private const string MultipleFactoryArgumentsMessage = "Factory \"{0}\" cannot have multiple generic types.";

    private readonly ILogger _logger;
    private readonly ContainerOptions _containerOptions;
    private readonly ObjectBindings _objectBindings;
    private readonly FactoryBindings _factoryBindings;
    private readonly HashSet<Type> _creatableTypes = new();

    public Validator(
        ILogger logger,
        ContainerOptions containerOptions,
        ObjectBindings objectBindings,
        FactoryBindings factoryBindings)
    {
        _logger = logger;
        _containerOptions = containerOptions;
        _objectBindings = objectBindings;
        _factoryBindings = factoryBindings;
    }

    public bool CanCreate(Type type)
    {
        if (_creatableTypes.Contains(type) || type.IsAssignableTo(typeof(ILogger)))
        {
            return true;
        }

        var parameters = ReflectionHelpers.GetConstructorParameters(type)
            .Where(s => s.GetCustomAttributes(typeof(InjectIgnoreAttribute), true).Length == 0)
            .ToArray();

        var canInject = parameters.All(p =>
        {
            if (ReflectionHelpers.HasTypeInDependencyTree(p.ParameterType, type))
            {
                throw new DependencyException(string.Format(CircularDependencyMessage, type));
            }

            return CanInjectAsObject(p.ParameterType) || CanInjectAsFactory(p.ParameterType);
        });

        if (!canInject)
        {
            return false;
        }

        _creatableTypes.Add(type);
        return true;
    }

    public void CheckForIllegalInjection(IBinding binding, Type? requestingType)
    {
        if (requestingType == null || !binding.AllowedInjectionTypes.Any())
        {
            return;
        }

        var isAllowed = binding.AllowedInjectionTypes.Any(a => a.IsAssignableFrom(requestingType));
        if (!isAllowed)
        {
            throw new DependencyException(string.Format(
                IllegalInjectionMessage,
                requestingType,
                binding.InterfaceType));
        }
    }

    public void CheckForCircularInjection(Type requestedType, object[] arguments)
    {
        var rootParameters = ReflectionHelpers.GetConstructorParameters(requestedType);
        var argumentTypes = arguments
            .Select(a => a.GetType())
            .ToArray();

        //ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var parameter in rootParameters)
        {
            var instanceExists = argumentTypes.Contains(parameter.ParameterType);
            var hasDefaultValue = !_containerOptions.InjectIntoDefaultProperties && parameter.HasDefaultValue;

            if (instanceExists || hasDefaultValue)
            {
                continue;
            }

            if (ReflectionHelpers.HasTypeInDependencyTree(parameter.ParameterType, requestedType))
            {
                throw new DependencyException(
                    string.Format(CircularDependencyMessage, requestedType));
            }
        }
    }

    private bool CanInjectAsObject(Type type)
    {
        var binding = _objectBindings.Find(type);
        if (binding is null)
        {
            return false;
        }

        return binding.Instance is not null || CanCreate(binding.ConcreteType);
    }

    private bool CanInjectAsFactory(Type type)
    {
        if (_factoryBindings.ContainsKey(type))
        {
            return true;
        }

        if (!_containerOptions.UseAutoFactories || !type.IsAssignableTo(typeof(IFactory)))
        {
            return false;
        }

        var genericTypes = type.GetGenericArguments();
        if (genericTypes.Length > 1)
        {
            _logger.LogWarning(string.Format(MultipleFactoryArgumentsMessage, type));
        }

        //TODO: Check for circular dependencies in factory
        return CanCreate(genericTypes[0]);
    }
}