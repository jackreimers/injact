namespace Injact.Core.Container;

public interface IDiContainer
{
    /// <summary>
    /// Create a binding using a concrete type.
    /// </summary>
    /// <typeparam name="TConcrete">The type of the object being bound.</typeparam>
    public ObjectBindingBuilder Bind<TConcrete>()
        where TConcrete : class;

    /// <summary>
    /// Create a binding using an interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface the object is being bound as.</typeparam>
    /// <typeparam name="TConcrete">The concrete type of the object being bound.</typeparam>
    public ObjectBindingBuilder Bind<TInterface, TConcrete>()
        where TConcrete : class, TInterface;

    /// <summary>
    /// Create a factory binding using a concrete type.
    /// </summary>
    /// <typeparam name="TFactory">The type of the factory being bound.</typeparam>
    /// <typeparam name="TObject">The type of object the factory creates.</typeparam>
    public FactoryBindingBuilder BindFactory<TFactory, TObject>()
        where TFactory : IFactory;

    /// <summary>
    /// Create a factory binding using an interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface the factory is being bound as.</typeparam>
    /// <typeparam name="TFactory">The type of the factory being bound.</typeparam>
    /// <typeparam name="TObject">The type of object the factory creates.</typeparam>
    public FactoryBindingBuilder BindFactory<TInterface, TFactory, TObject>()
        where TInterface : IFactory
        where TFactory : TInterface;

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <typeparam name="TInterface">The type to be resolved.</typeparam>
    /// <param name="requestingType">The type of the object requesting an instance of this type.</param>
    public TInterface Resolve<TInterface>(Type requestingType)
        where TInterface : class;

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <typeparam name="TInterface">The type to be resolved.</typeparam>
    /// <param name="requestingType">The type of the object requesting an instance of this type.</param>
    /// <param name="throwOnNotFound">If true, an exception will be thrown if the type is not found.</param>
    public TInterface? Resolve<TInterface>(Type requestingType, bool throwOnNotFound)
        where TInterface : class;

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <typeparam name="TInterface">The type to be resolved.</typeparam>
    /// <param name="requestingObject">The object requesting an instance of this type.</param>
    public TInterface Resolve<TInterface>(object requestingObject)
        where TInterface : class;

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <param name="requestingObject">The object requesting an instance of this type.</param>
    /// <param name="throwOnNotFound">If true, an exception will be thrown if the type is not found.</param>
    public TInterface? Resolve<TInterface>(object requestingObject, bool throwOnNotFound)
        where TInterface : class;

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <param name="requestedType">The type to be resolved.</param>
    /// <param name="requestingType">The type of the object requesting an instance of this type.</param>
    public object Resolve(Type requestedType, Type requestingType);

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <param name="requestedType">The type to be resolved.</param>
    /// <param name="requestingType">The type of the object requesting an instance of this type.</param>
    /// <param name="throwOnNotFound">If true, an exception will be thrown if the type is not found.</param>
    public object? Resolve(
        Type requestedType,
        Type requestingType,
        bool throwOnNotFound);

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <param name="requestedType">The type to be resolved.</param>
    /// <param name="requestingObject">The object requesting an instance of this type.</param>
    public object Resolve(Type requestedType, object requestingObject);

    /// <summary>
    /// Resolve a type from the container.
    /// </summary>
    /// <param name="requestedType">The type to be resolved.</param>
    /// <param name="requestingObject">The object requesting an instance of this type.</param>
    /// <param name="throwOnNotFound">If true, an exception will be thrown if the type is not found.</param>
    public object? Resolve(
        Type requestedType,
        object requestingObject,
        bool throwOnNotFound);

    /// <summary>
    /// Add options from a JSON file to the container.
    /// </summary>
    /// <param name="section">The name of the section in the JSON file.</param>
    /// <param name="path">The JSON file path relative to the project root.</param>
    /// <typeparam name="T">The options object to deserialise into.</typeparam>
    /// <remarks>Method will search for appsettings.json in the project root if path is null.</remarks>
    public void AddOptions<T>(string? section = null, string? path = null);

    /// <summary>
    /// Create a new instance of a type.
    /// </summary>
    /// <param name="requestedType">The type to be created.</param>
    /// <param name="arguments">Arguments to be passed to the constructor of the created object.</param>
    /// <remarks>It's recommended to use a factory instead of calling this method directly.</remarks>
    public object Create(Type requestedType, params object[] arguments);

    /// <summary>
    /// Create a new instance of a type.
    /// </summary>
    /// <param name="requestedType">The type to be created.</param>
    /// <param name="deferInitialisation">
    /// If true and the object implements <see cref="ILifecycleObject" />, the container will
    /// not call Awake or Start.
    /// </param>
    /// <param name="arguments">Arguments to be passed to the constructor of the created object.</param>
    /// <remarks>It's recommended to use a factory instead of calling this method directly.</remarks>
    public object Create(
        Type requestedType,
        bool deferInitialisation = false,
        params object[] arguments);
}