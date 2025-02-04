namespace Injact.Core.Container.Components;

internal class Validator
{
    private const string CannotCreateInterfaceInstanceMessage = "Cannot create an instance of an interface.";
    private const string ValidationFailedMessage = "Type \"{0}\" cannot be created by the container.";
    private const string IllegalInjectionMessage = "\"{0}\" requested type of \"{1}\" when it is not allowed to.";
    private const string CircularInjectionMessage = "Requested type of {0} contains a circular dependency.";
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

    public bool CanInject(Type type)
    {
        return CanInjectAsObject(type) || CanInjectAsFactory(type);
    }

    public bool CanCreate(Type type)
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
                 CanInjectAsFactory(s.ParameterType));

        if (!canCreateType)
        {
            throw new DependencyException(string.Format(ValidationFailedMessage, type));
        }

        _creatableTypes.Add(type);

        return canCreateType;
    }

    public ArgumentCheckResult ValidateArguments(Type requestedType, params object[] arguments)
    {
        var typedArguments = arguments.ToDictionary(s => s.GetType(), s => s);
        var typedArgumentsWithInterfaces = typedArguments.ToDictionary(s => s.Key, s => s.Value);
        var ignoredInterfaces = new List<Type>();

        Guard.Against.Condition(
            arguments.Length != typedArguments.Count,
            string.Format(ValidationFailedMessage, requestedType));

        foreach (var type in typedArguments)
        {
            //Check for duplicate interface implementations and exclude them from injection
            foreach (var implemented in type.Key.GetInterfaces())
            {
                if (ignoredInterfaces.Contains(implemented))
                {
                    continue;
                }

                if (typedArgumentsWithInterfaces.ContainsKey(implemented))
                {
                    typedArgumentsWithInterfaces.Remove(implemented);
                    ignoredInterfaces.Add(implemented);

                    continue;
                }

                typedArgumentsWithInterfaces.Add(implemented, type.Value);
            }
        }

        var constructor = Guard.Against.Null(
            ReflectionHelpers.GetConstructor(requestedType, arguments.Select(s => s.GetType())));

        var parameterInfos = constructor.GetParameters();
        var parameters = new Dictionary<ParameterInfo, object?>();

        foreach (var parameterInfo in parameterInfos)
        {
            if (!_containerOptions.InjectIntoDefaultProperties && parameterInfo.HasDefaultValue)
            {
                parameters.Add(parameterInfo, Type.Missing);
                continue;
            }

            var type = parameterInfo.ParameterType;
            var targetType = typedArgumentsWithInterfaces
                .Where(s => s.Key.IsAssignableTo(type))
                .Select(s => s.Value)
                .FirstOrDefault();

            parameters.Add(parameterInfo, targetType);
        }

        return new ArgumentCheckResult(constructor, parameters);
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
            throw new DependencyException(string.Format(IllegalInjectionMessage, requestingType, binding.InterfaceType));
        }
    }

    public void CheckForCircularInjection(Type requestedType, object[] arguments)
    {
        var rootParameters = ReflectionHelpers.GetConstructorParameters(requestedType);
        var argumentTypes = arguments
            .Select(arg => arg.GetType())
            .ToArray();

        //ReSharper disable once LoopCanBeConvertedToQuery
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
                throw new DependencyException(string.Format(CircularInjectionMessage, requestedType));
            }
        }
    }

    private bool CanInjectAsObject(Type type)
    {
        return type.IsAssignableTo(typeof(ILogger)) ||
               _objectBindings.ContainsKey(type) ||
               _objectBindings.Count(b => type.IsAssignableTo(b.Key)) == 1;
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

        return CanCreate(genericTypes[0]);
    }

    public record ArgumentCheckResult(
        ConstructorInfo Constructor,
        Dictionary<ParameterInfo, object?> Parameters);
}