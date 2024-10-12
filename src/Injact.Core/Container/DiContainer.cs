namespace Injact.Container;

public class ObjectBindings : Dictionary<Type, ObjectBinding> { }

public class FactoryBindings : Dictionary<Type, FactoryBinding> { }

public class DiContainer : IDiContainer
{
    private const string TypeAlreadyBoundErrorMessage = "Type \"{0}\" is already bound.";
    private const string ObjectCannotBeBoundErrorMessage = "Cannot bind type \"{0}\" as factory.";
    private const string FactoryCannotBeBoundErrorMessage = "Cannot bind factory \"{0}\" as object.";
    private const string TwoOrMoreImplementationsErrorMessage = "\"{0}\" is implemented by two or more provided arguments and will be ignored.";
    private const string DuplicateArgumentTypesErrorMessage = "Cannot pass duplicate argument types to create method.";

    private const string ResolveFailedForTypeErrorMessage = "Failed to resolve type \"{0}\".";
    private const string CreateFailedErrorMessage = "Failed to create type \"{0}\".";
    private const string CannotCreateInterfaceInstanceErrorMessage = "Cannot create an instance of an interface.";

    private const string JsonNoFileFoundErrorMessage = "No JSON file found at \"{0}\".";
    private const string JsonNoSectionFoundErrorMessage = "Section \"{0}\" not found in JSON file.";
    private const string JsonNoOptionsFoundForSectionErrorMessage = "No options found for section \"{0}\".";
    private const string JsonNoOptionsLoadedForSectionErrorMessage = "Failed to load options for section \"{0}\".";

    private const string ImmediateBindingWasCreatedMessage = "Immediate binding for type \"{0}\" was created.";

    private readonly ILogger _logger;
    private readonly ContainerOptions _containerOptions;
    private readonly Injector _injector;
    private readonly Validator _validator;
    private readonly ObjectBindings _objectBindings = new();
    private readonly ObjectBindings _deferredBindings = new();
    private readonly FactoryBindings _factoryBindings = new();

    private bool isCheckingImmediateBindings;

    /// <summary>
    /// Create a new instance of the dependency injection container.
    /// </summary>
    public DiContainer()
        : this(new ContainerOptions { LoggingProvider = new DefaultLoggingProvider() }) { }

    /// <summary>
    /// Create a new instance of the dependency injection container.
    /// </summary>
    /// <param name="containerOptions">Settings to control container features.</param>
    public DiContainer(ContainerOptions containerOptions)
    {
        _logger = containerOptions.LoggingProvider.GetLogger<DiContainer>(containerOptions);
        _containerOptions = containerOptions;
        _injector = new Injector(this);
        _validator = new Validator(
            _logger,
            _containerOptions,
            _objectBindings,
            _factoryBindings);

        Bind<DiContainer>()
            .FromInstance(this);

        Bind<Injector>()
            .FromInstance(_injector);

        Bind<EditorValueMapper>()
            .AsSingleton();

        Bind<IProfiler>()
            .FromInstance(new Profiler(_logger));

        _logger.LogInformation("Dependency injection container initialised.");
    }

    /// <inheritdoc />
    public ObjectBindingBuilder Bind<TConcrete>()
        where TConcrete : class
    {
        return BindObjectInternal<TConcrete, TConcrete>();
    }

    /// <inheritdoc />
    public ObjectBindingBuilder Bind<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        return BindObjectInternal<TInterface, TConcrete>();
    }

    /// <inheritdoc />
    public FactoryBindingBuilder BindFactory<TFactory, TObject>()
        where TFactory : IFactory
    {
        return BindFactoryInternal<TFactory, TFactory, TObject>();
    }

    /// <inheritdoc />
    public FactoryBindingBuilder BindFactory<TInterface, TFactory, TObject>()
        where TInterface : IFactory
        where TFactory : TInterface
    {
        return BindFactoryInternal<TInterface, TFactory, TObject>();
    }

    /// <inheritdoc />
    public TInterface Resolve<TInterface>(Type requestingType)
        where TInterface : class
    {
        var resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingType);
        if (resolved == null)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public TInterface? Resolve<TInterface>(Type requestingType, bool throwOnNotFound)
        where TInterface : class
    {
        var resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingType);
        if (resolved == null && throwOnNotFound)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public TInterface Resolve<TInterface>(object requestingObject)
        where TInterface : class
    {
        var resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType());
        if (resolved == null)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public TInterface? Resolve<TInterface>(object requestingObject, bool throwOnNotFound)
        where TInterface : class
    {
        var resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType());
        if (resolved == null && throwOnNotFound)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object Resolve(Type requestedType, Type requestingType)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingType);
        if (resolved == null)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object? Resolve(Type requestedType, Type requestingType, bool throwOnNotFound)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingType);
        if (resolved == null && throwOnNotFound)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object Resolve(Type requestedType, object requestingObject)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingObject.GetType());
        if (resolved == null)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object? Resolve(Type requestedType, object requestingObject, bool throwOnNotFound)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingObject.GetType());
        if (resolved == null && throwOnNotFound)
        {
            throw new DependencyException(string.Format(ResolveFailedForTypeErrorMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public void AddOptions<T>(string? section = null, string? path = null)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var appsettingsPath = !string.IsNullOrWhiteSpace(path)
            ? Path.Combine(workingDirectory, path)
            : Path.Combine(workingDirectory, "appsettings.json");

        if (!File.Exists(appsettingsPath))
        {
            throw new OptionsExeption(string.Format(JsonNoFileFoundErrorMessage, appsettingsPath));
        }

        try
        {
            var file = File.ReadAllText(appsettingsPath);
            var json = JsonDocument.Parse(file);
            var value = json.RootElement;

            if (section != null)
            {
                var properties = section.Split(':');
                foreach (var property in properties)
                {
                    if (!value.TryGetProperty(property, out var next))
                    {
                        throw new OptionsExeption(string.Format(JsonNoSectionFoundErrorMessage, section));
                    }

                    value = next;
                }
            }

            //TODO: This is failing silently when the section does not exist in the file
            var options = value.Deserialize<T>();
            if (options == null)
            {
                throw new OptionsExeption(string.Format(JsonNoOptionsFoundForSectionErrorMessage, section));
            }

            Bind<IOptions<T>>()
                .FromInstance(new Options<T>(options))
                .AsSingleton();
        }

        catch (Exception exception)
        {
            throw new OptionsExeption(
                string.Format(JsonNoOptionsLoadedForSectionErrorMessage, section),
                exception);
        }
    }

    /// <inheritdoc />
    public object Create(Type requestedType, params object[] arguments)
    {
        return CreateInternal(requestedType, false, false, arguments)
               ?? throw new DependencyException(string.Format(CreateFailedErrorMessage, requestedType.Name));
    }

    /// <inheritdoc />
    public object Create(
        Type requestedType,
        bool deferInitialisation = false,
        bool throwOnNotFound = false,
        params object[] arguments)
    {
        return CreateInternal(requestedType, deferInitialisation, throwOnNotFound, arguments)
               ?? throw new DependencyException(string.Format(CreateFailedErrorMessage, requestedType.Name));
    }

    private ObjectBindingBuilder BindObjectInternal<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        Guard.Against.Condition(
            _objectBindings.ContainsKey(typeof(TInterface)),
            string.Format(TypeAlreadyBoundErrorMessage, typeof(TInterface)));

        Guard.Against.Assignable<TInterface, IFactory>(
            string.Format(FactoryCannotBeBoundErrorMessage, typeof(TInterface)));

        Guard.Against.Assignable<TConcrete, IFactory>(
            string.Format(ObjectCannotBeBoundErrorMessage, typeof(TConcrete)));

        var binding = new ObjectBinding(typeof(TInterface), typeof(TConcrete));
        _objectBindings.Add(typeof(TInterface), binding);

        foreach (var deferred in _deferredBindings)
        {
            _objectBindings.Add(deferred.Key, deferred.Value);
        }

        _deferredBindings.Clear();
        return new ObjectBindingBuilder(binding);
    }

    private FactoryBindingBuilder BindFactoryInternal<TInterface, TFactory, TObject>()
        where TInterface : IFactory
        where TFactory : TInterface
    {
        Guard.Against.Condition(
            _factoryBindings.ContainsKey(typeof(TInterface)),
            string.Format(TypeAlreadyBoundErrorMessage, typeof(TInterface)));

        Guard.Against.Condition(
            _factoryBindings.ContainsKey(typeof(TInterface)),
            string.Format(TypeAlreadyBoundErrorMessage, typeof(TInterface)));

        Guard.Against.Condition(
            _factoryBindings.ContainsKey(typeof(TFactory)),
            string.Format(TypeAlreadyBoundErrorMessage, typeof(TFactory)));

        var binding = new FactoryBinding(typeof(TInterface), typeof(TFactory), typeof(TObject));
        _factoryBindings.Add(typeof(TInterface), binding);

        foreach (var deferred in _deferredBindings)
        {
            _objectBindings.Add(deferred.Key, deferred.Value);
        }

        _deferredBindings.Clear();

        return new FactoryBindingBuilder(binding);
    }

    private TInterface? ResolveInternal<TInterface>(Type requestedType, Type requestingType, bool throwOnNotFound = true)
        where TInterface : class
    {
        Guard.Against.CircularInjection(
            _containerOptions,
            _objectBindings,
            requestedType,
            Array.Empty<object>());

        return !requestedType.IsAssignableTo(typeof(IFactory))
            ? ResolveObject<TInterface>(requestedType, requestingType, throwOnNotFound)
            : ResolveFactory<TInterface>(requestedType, requestingType);
    }

    private TInterface? ResolveObject<TInterface>(Type requestedType, Type requestingType, bool throwOnNotFound)
        where TInterface : class
    {
        OnResolve();

        if (requestedType.IsAssignableTo(typeof(ILogger)))
        {
            var loggerType = _containerOptions.LoggingProvider
                .GetLoggerType()
                .MakeGenericType(requestingType);

            if (_objectBindings.TryGetValue(loggerType, out var loggerBinding))
            {
                return loggerBinding.Instance as TInterface;
            }

            var constructor = loggerType
                .GetConstructors()
                .FirstOrDefault();

            var instance = Guard.Against.Null(constructor?.Invoke(new object[] { _containerOptions }));
            var newBinding = new ObjectBinding(loggerType, loggerType, instance);

            newBinding.Lock();
            _objectBindings.Add(loggerType, newBinding);

            return Guard.Against.Null(instance as TInterface);
        }

        if (_objectBindings.TryGetValue(requestedType, out var binding))
        {
            Guard.Against.IllegalInjection(binding, requestingType);

            if (binding.Instance != null)
            {
                return Guard.Against.Null(binding.Instance as TInterface);
            }

            var created = CreateInternal(binding.ConcreteType, false, throwOnNotFound);
            if (created == null)
            {
                return default;
            }

            if (binding.IsSingleton)
            {
                binding.Instance = created;
            }

            return Guard.Against.Null(created as TInterface);
        }

        return default;
    }

    private TInterface? ResolveFactory<TInterface>(Type requestedType, Type requestingType)
        where TInterface : class
    {
        _factoryBindings.TryGetValue(requestedType, out var factoryBinding);

        if (_containerOptions.UseAutoFactories && factoryBinding == null)
        {
            var type = requestedType
                .GetGenericArguments()
                .First();

            var factoryType = typeof(Factory<>).MakeGenericType(type);
            return Guard.Against.Null(Create(factoryType) as TInterface);
        }

        if (factoryBinding == null)
        {
            return default;
        }

        Guard.Against.IllegalInjection(factoryBinding, requestingType);
        return Guard.Against.Null(Create(factoryBinding.ConcreteType) as TInterface);
    }

    private void OnResolve()
    {
        if (isCheckingImmediateBindings)
        {
            return;
        }

        isCheckingImmediateBindings = true;
        var immediateBindings = _objectBindings.Where(s => s.Value is { IsImmediate: true, IsLocked: false });

        foreach (var binding in immediateBindings)
        {
            TryProcessImmediateObjectBinding(binding.Value);
        }

        isCheckingImmediateBindings = false;
    }

    private void TryProcessImmediateObjectBinding(ObjectBinding binding)
    {
        var canCreateObject = _validator.CanCreate(binding.ConcreteType, false);
        if (!canCreateObject)
        {
            _objectBindings.Remove(binding.InterfaceType);
            _deferredBindings.Add(binding.InterfaceType, binding);

            return;
        }

        var created = CreateInternal(binding.ConcreteType, false, false);
        if (created == null)
        {
            _objectBindings.Remove(binding.InterfaceType);
            _deferredBindings.Add(binding.InterfaceType, binding);

            return;
        }

        binding.Instance = created;
        _logger.LogInformation(string.Format(ImmediateBindingWasCreatedMessage, binding.InterfaceType));
    }

    private object? CreateInternal(Type requestedType, bool deferInitialisation, bool throwOnNotFound, params object[] arguments)
    {
        Guard.Against.Condition(requestedType.IsInterface, CannotCreateInterfaceInstanceErrorMessage);
        Guard.Against.CircularInjection(_containerOptions, _objectBindings, requestedType, arguments);

        if (!_validator.CanCreate(requestedType, throwOnNotFound))
        {
            return null;
        }

        var argumentResult = ValidateCreationArguments(requestedType, arguments);
        var missingParameters = argumentResult.Parameters.Where(s => s.Value == null).ToArray();

        foreach (var parameter in missingParameters)
        {
            var dependencyResult = _validator.CanCreate(parameter.Key.ParameterType, throwOnNotFound);
            if (!dependencyResult)
            {
                return null;
            }

            argumentResult.Parameters[parameter.Key] = Resolve(parameter.Key.ParameterType, this);
        }

        var argumentValues = argumentResult.Parameters.Values.ToArray();
        var constructed = argumentResult.Constructor.Invoke(argumentValues);

        _injector.InjectInto(constructed);

        if (constructed is not ILifecycleObject lifecycleObject)
        {
            return constructed;
        }

        var lifecycleType = typeof(LifecycleObject);

        var updateMethod = Guard.Against.Null(requestedType.GetMethod("Update"));
        if (updateMethod.DeclaringType != lifecycleType)
        {
            var property = lifecycleType.GetField("_shouldRunUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
            {
                property.SetValue(constructed, true);
            }
        }

        if (deferInitialisation)
        {
            return constructed;
        }

        lifecycleObject.Awake();
        lifecycleObject.Start();
        lifecycleObject.Enable();

        return lifecycleObject;
    }

    private ArgumentCheckResult ValidateCreationArguments(Type requestedType, params object[] arguments)
    {
        var typedArguments = arguments.ToDictionary(s => s.GetType(), s => s);
        var typedArgumentsWithInterfaces = typedArguments.ToDictionary(s => s.Key, s => s.Value);
        var ignoredInterfaces = new List<Type>();

        Guard.Against.Condition(
            arguments.Length != typedArguments.Count,
            DuplicateArgumentTypesErrorMessage);

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
                    _logger.LogInformation(string.Format(TwoOrMoreImplementationsErrorMessage, implemented.Name));

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

    private record ArgumentCheckResult(ConstructorInfo Constructor, Dictionary<ParameterInfo, object?> Parameters);
}