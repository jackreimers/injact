namespace Injact;

internal static class ReflectionHelpers
{
    public static ConstructorInfo GetConstructor(Type type)
    {
        //TODO: Should the constructor with the largest number of parameters be used instead?
        var constructors = type.GetConstructors();
        var defaultConstructor = constructors[0];

        foreach (var constructor in constructors)
        {
            //Preference is given to the constructor with the Inject attribute
            if (!constructor.GetCustomAttributes(typeof(InjectAttribute), true).Any())
                continue;

            defaultConstructor = constructor;
            break;
        }

        return defaultConstructor;
    }

    public static IEnumerable<ParameterInfo> GetParameters(Type type)
    {
        var constructor = GetConstructor(type);
        return constructor.GetParameters();
    }

    public static FieldInfo[] GetAllFields(Type type)
    {
        Guard.Against.Null(type, "Cannot get fields from null type!");
        var fields = new List<FieldInfo>();

        while (true)
        {
            fields.AddRange(
                type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var baseType = type.BaseType;
            if (baseType == null)
                break;

            type = baseType;
        }

        return fields.ToArray();
    }

    public static FieldInfo GetBackingField(Type type, string parameterName)
    {
        Guard.Against.Null(type, "Cannot get field from null type!");
        Guard.Against.NullOrWhitespace(parameterName, "Cannot get backing field from null or empty parameter name!");

        while (true)
        {
            var field = type.GetField($"<{parameterName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            var baseType = type.BaseType;
            if (baseType == null)
                break;

            type = baseType;
        }

        throw new DependencyException($"Could not find backing field for parameter {parameterName} on type {type}!");
    }
}