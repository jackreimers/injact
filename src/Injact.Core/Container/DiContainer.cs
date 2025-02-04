namespace Injact.Core.Container;

public class ObjectBindings : Dictionary<Type, ObjectBinding> { }

public class FactoryBindings : Dictionary<Type, FactoryBinding> { }

public class DiContainer : IDiContainer
{
    private const string TypeAlreadyBoundMessage = "Type \"{0}\" is already bound.";
    private const string ObjectCannotBeBoundMessage = "Cannot bind type \"{0}\" as factory.";
    private const string FactoryCannotBeBoundMessage = "Cannot bind factory \"{0}\" as object.";
    private const string ResolveFailedMessage = "Failed to resolve type \"{0}\".";
    private const string CreateFailedMessage = "Failed to create type \"{0}\".";
    private const string JsonNoFileFoundMessage = "No JSON file found at \"{0}\".";
    private const string JsonNoSectionFoundMessage = "Section \"{0}\" not found in JSON file.";
    private const string JsonNoOptionsFoundForSectionMessage = "No options found for section \"{0}\".";
    private const string JsonNoOptionsLoadedForSectionMessage = "Failed to load options for section \"{0}\".";
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

        Bind<IDiContainer, DiContainer>()
            .FromInstance(this);

        Bind<IInjector, Injector>()
            .FromInstance(_injector);

        Bind<IProfiler, Profiler>()
            .FromInstance(new Profiler(_logger));

        Bind<EditorValueMapper>()
            .AsSingleton();
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
            throw new DependencyException(
                string.Format(ResolveFailedMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public TInterface? Resolve<TInterface>(Type requestingType, bool throwOnNotFound)
        where TInterface : class
    {
        TInterface? resolved = null;

        try
        {
            resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingType);
            if (resolved == null && throwOnNotFound)
            {
                throw new DependencyException(
                    string.Format(ResolveFailedMessage, typeof(TInterface)));
            }
        }

        catch (Exception)
        {
            if (throwOnNotFound)
            {
                throw;
            }
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
            throw new DependencyException(
                string.Format(ResolveFailedMessage, typeof(TInterface)));
        }

        return resolved;
    }

    /// <inheritdoc />
    public TInterface? Resolve<TInterface>(object requestingObject, bool throwOnNotFound)
        where TInterface : class
    {
        TInterface? resolved = null;

        try
        {
            resolved = ResolveInternal<TInterface>(typeof(TInterface), requestingObject.GetType());
            if (resolved == null && throwOnNotFound)
            {
                throw new DependencyException(
                    string.Format(ResolveFailedMessage, typeof(TInterface)));
            }
        }

        catch (Exception)
        {
            if (throwOnNotFound)
            {
                throw;
            }
        }

        return resolved;
    }

    /// <inheritdoc />
    public object Resolve(Type requestedType, Type requestingType)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingType);
        if (resolved == null)
        {
            throw new DependencyException(
                string.Format(ResolveFailedMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object? Resolve(
        Type requestedType,
        Type requestingType,
        bool throwOnNotFound)
    {
        object? resolved = null;

        try
        {
            resolved = ResolveInternal<object>(requestedType, requestingType);
            if (resolved == null && throwOnNotFound)
            {
                throw new DependencyException(
                    string.Format(ResolveFailedMessage, requestedType));
            }
        }

        catch (Exception)
        {
            if (throwOnNotFound)
            {
                throw;
            }
        }

        return resolved;
    }

    /// <inheritdoc />
    public object Resolve(Type requestedType, object requestingObject)
    {
        var resolved = ResolveInternal<object>(requestedType, requestingObject.GetType());
        if (resolved == null)
        {
            throw new DependencyException(
                string.Format(ResolveFailedMessage, requestedType));
        }

        return resolved;
    }

    /// <inheritdoc />
    public object? Resolve(
        Type requestedType,
        object requestingObject,
        bool throwOnNotFound)
    {
        object? resolved = null;

        try
        {
            resolved = ResolveInternal<object>(requestedType, requestingObject.GetType());
            if (resolved == null)
            {
                throw new DependencyException(
                    string.Format(ResolveFailedMessage, requestedType));
            }
        }

        catch (Exception)
        {
            if (throwOnNotFound)
            {
                throw;
            }
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
            throw new OptionsExeption(string.Format(JsonNoFileFoundMessage, appsettingsPath));
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
                        throw new OptionsExeption(
                            string.Format(JsonNoSectionFoundMessage, section));
                    }

                    value = next;
                }
            }

            //TODO: This is failing silently when the section does not exist in the file
            var options = value.Deserialize<T>();
            if (options == null)
            {
                throw new OptionsExeption(
                    string.Format(JsonNoOptionsFoundForSectionMessage, section));
            }

            Bind<IOptions<T>>()
                .FromInstance(new Options<T>(options))
                .AsSingleton();
        }

        catch (Exception exception)
        {
            throw new OptionsExeption(
                string.Format(JsonNoOptionsLoadedForSectionMessage, section),
                exception);
        }
    }

    /// <inheritdoc />
    public object Create(Type requestedType, params object[] arguments)
    {
        var created = CreateInternal(
            requestedType,
            false,
            arguments);

        if (created is null)
        {
            throw new DependencyException(
                string.Format(CreateFailedMessage, requestedType.Name));
        }

        return created;
    }

    /// <inheritdoc />
    public object Create(
        Type requestedType,
        bool deferInitialisation = false,
        params object[] arguments)
    {
        var created = CreateInternal(
            requestedType,
            deferInitialisation,
            arguments);

        if (created is null)
        {
            throw new DependencyException(
                string.Format(CreateFailedMessage, requestedType.Name));
        }

        return created;
    }

    private ObjectBindingBuilder BindObjectInternal<TInterface, TConcrete>()
        where TConcrete : class, TInterface
    {
        Guard.Against.Condition(
            _objectBindings.ContainsKey(typeof(TInterface)),
            string.Format(TypeAlreadyBoundMessage, typeof(TInterface)));

        Guard.Against.Assignable<TInterface, IFactory>(
            string.Format(FactoryCannotBeBoundMessage, typeof(TInterface)));

        Guard.Against.Assignable<TConcrete, IFactory>(
            string.Format(ObjectCannotBeBoundMessage, typeof(TConcrete)));

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
            string.Format(TypeAlreadyBoundMessage, typeof(TInterface)));

        Guard.Against.Condition(
            _factoryBindings.ContainsKey(typeof(TInterface)),
            string.Format(TypeAlreadyBoundMessage, typeof(TInterface)));

        Guard.Against.Condition(
            _factoryBindings.ContainsKey(typeof(TFactory)),
            string.Format(TypeAlreadyBoundMessage, typeof(TFactory)));

        var binding = new FactoryBinding(typeof(TInterface), typeof(TFactory), typeof(TObject));
        _factoryBindings.Add(typeof(TInterface), binding);

        foreach (var deferred in _deferredBindings)
        {
            _objectBindings.Add(deferred.Key, deferred.Value);
        }

        _deferredBindings.Clear();

        return new FactoryBindingBuilder(binding);
    }

    private TInterface? ResolveInternal<TInterface>(
        Type requestedType,
        Type requestingType,
        bool throwOnNotFound = true
    )
        where TInterface : class
    {
        CheckBindings();

        _validator.CheckForCircularInjection(
            requestedType,
            Array.Empty<object>());

        return !requestedType.IsAssignableTo(typeof(IFactory))
            ? ResolveObject<TInterface>(requestedType, requestingType, throwOnNotFound)
            : ResolveFactory<TInterface>(requestedType, requestingType);
    }

    private TInterface? ResolveObject<TInterface>(
        Type requestedType,
        Type requestingType,
        bool throwOnNotFound
    )
        where TInterface : class
    {
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

        var exactMatch = _objectBindings.TryGetValue(requestedType, out var binding);
        if (!exactMatch)
        {
            var assignable = _objectBindings
                .Where(b => requestedType.IsAssignableTo(b.Key))
                .Select(b => b.Value)
                .ToArray();

            if (assignable.Length == 1)
            {
                binding = assignable.First();
            }
        }

        if (binding is null)
        {
            return null;
        }

        _validator.CheckForIllegalInjection(binding, requestingType);
        if (binding.Instance != null)
        {
            return Guard.Against.Null(binding.Instance as TInterface);
        }

        var created = CreateInternal(binding.ConcreteType, false, throwOnNotFound);
        if (created == null)
        {
            return null;
        }

        if (binding.IsSingleton)
        {
            binding.Instance = created;
        }

        return Guard.Against.Null(created as TInterface);
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
            return null;
        }

        _validator.CheckForIllegalInjection(factoryBinding, requestingType);
        return Guard.Against.Null(Create(factoryBinding.ConcreteType) as TInterface);
    }

    private object? CreateInternal(
        Type requestedType,
        bool deferInitialisation,
        params object[] arguments)
    {
        _validator.CheckForCircularInjection(requestedType, arguments);
        if (!_validator.CanCreate(requestedType))
        {
            return null;
        }

        var argumentResult = _validator.ValidateArguments(requestedType, arguments);
        var missingParameters = argumentResult.Parameters
            .Where(s => s.Value == null)
            .ToArray();

        foreach (var parameter in missingParameters)
        {
            if (!_validator.CanInject(parameter.Key.ParameterType))
            {
                var dependencyResult = _validator.CanCreate(parameter.Key.ParameterType);
                if (!dependencyResult)
                {
                    return null;
                }
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

    private void CheckBindings()
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
        var canCreateObject = _validator.CanCreate(binding.ConcreteType);
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
}