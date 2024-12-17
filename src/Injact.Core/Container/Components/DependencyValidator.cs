namespace Injact.Core.Container.Components;

internal class DependencyValidator
{
    private const string CannotCreateInterfaceInstanceMessage = "Cannot create an instance of an interface.";
    private const string ValidationFailedForTypeMessage = "Type \"{0}\" failed validation and cannot be created by the container.";
    private const string IllegalInjectionMessage = "\"{0}\" requested type of \"{1}\" when it is not allowed to.";
    private const string CircularInjectionMessage = "Requested type of {0} contains a circular dependency.";

    private readonly ILogger _logger;
    private readonly ContainerOptions _containerOptions;
    private readonly ObjectBindings _objectBindings;
    private readonly FactoryBindings _factoryBindings;
    private readonly HashSet<Type> _creatableTypes = new();

    public DependencyValidator(
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

    public bool CanCreate(Type type, bool throwOnNotFound)
    {
        if (type.IsInterface)
        {
            throw new DependencyException(CannotCreateInterfaceInstanceMessage);
        }

        if (_creatableTypes.Contains(type))
        {
            return true;
        }

        var parameters = ReflectionHelpers.GetConstructorParameters(type)
            .Where(s => s.GetCustomAttributes(typeof(InjectIgnoreAttribute), true).Length == 0);

        var canCreateType = parameters.All(
            s => CanInjectAsObject(s.ParameterType) ||
                 CanInjectAsFactory(s.ParameterType, throwOnNotFound));

        if (!canCreateType)
        {
            if (throwOnNotFound)
            {
                throw new InvalidOperationException(
                    string.Format(ValidationFailedForTypeMessage, type));
            }
        }

        else
        {
            _creatableTypes.Add(type);
        }

        return canCreateType;
    }

    public void CheckForIllegalInjection(IBinding binding, Type? requestingType)
    {
        if (requestingType == null || !binding.AllowedInjectionTypes.Any())
        {
            return;
        }

        var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
        if (!isAllowed)
        {
            throw new DependencyException(
                string.Format(IllegalInjectionMessage, requestingType, binding.InterfaceType));
        }
    }

    public void CheckForCircularInjection(Type requestedType, object[] arguments)
    {
        var rootParameters = ReflectionHelpers.GetConstructorParameters(requestedType);
        var argumentTypes = arguments
            .Select(arg => arg.GetType())
            .ToArray();

        foreach (var parameter in rootParameters)
        {
            var bindingExists = _objectBindings.ContainsKey(parameter.ParameterType);
            var instanceExists = argumentTypes.Contains(parameter.ParameterType);
            var hasDefaultValue = !_containerOptions.InjectIntoDefaultProperties && parameter.HasDefaultValue;

            if (bindingExists || instanceExists || hasDefaultValue)
            {
                continue;
            }

            if (ReflectionHelpers.HasTypeInDependencyTree(parameter.ParameterType, requestedType))
            {
                throw new DependencyException(
                    string.Format(CircularInjectionMessage, requestedType));
            }
        }
    }

    private bool CanInjectAsObject(Type type)
    {
        return _objectBindings.ContainsKey(type) || type.IsAssignableTo(typeof(ILogger));
    }

    private bool CanInjectAsFactory(Type type, bool throwOnNotFound)
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
            _logger.LogWarning(
                $"Factory \"{type.Name}\" has multiple generic types which is not supported.");
        }

        return CanCreate(genericTypes[0], throwOnNotFound);
    }
}