//ReSharper disable MemberCanBePrivate.Global

namespace Injact.Core.Utilities.Helpers;

internal static class ReflectionHelpers
{
    private const string BackingFieldNotFoundMessage = "Could not find backing field for parameter {0} on type {1}.";

    public static bool IsPrimitive(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
    }

    public static bool HasTypeInDependencyTree(Type type, Type dependencyType)
    {
        if (IsPrimitive(type) || type.IsInterface || type.IsAbstract)
        {
            return false;
        }

        if (type == dependencyType)
        {
            return true;
        }

        return GetConstructorParameters(type)
            .Any(parameter => HasTypeInDependencyTree(parameter.ParameterType, dependencyType));
    }

    public static ConstructorInfo? GetConstructor(Type type)
    {
        var constructors = type.GetConstructors();
        if (!constructors.Any())
        {
            return null;
        }

        var defaultConstructor = constructors[0];

        foreach (var constructor in constructors)
        {
            //Preference is given to the constructor with the Inject attribute
            if (!constructor.GetCustomAttributes(typeof(InjectAttribute), true).Any())
            {
                continue;
            }

            defaultConstructor = constructor;
            break;
        }

        return defaultConstructor;
    }

    public static ConstructorInfo? GetConstructor(Type type, IEnumerable<Type> parameterTypes)
    {
        var constructors = type.GetConstructors();
        if (!constructors.Any())
        {
            return null;
        }

        var defaultConstructor = constructors[0];
        var defaultMatchCount = 0;

        foreach (var constructor in constructors)
        {
            //Preference is given to the constructor with the Inject attribute
            if (constructor.GetCustomAttributes(typeof(InjectAttribute), true).Any())
            {
                defaultConstructor = constructor;
                break;
            }

            //TODO: This does not account for assignable types
            var parameters = constructor.GetParameters();
            var matchCount = parameters.Count(parameter => parameterTypes.Contains(parameter.ParameterType));
            if (matchCount <= defaultMatchCount)
            {
                continue;
            }

            defaultConstructor = constructor;
            defaultMatchCount = matchCount;
        }

        return defaultConstructor;
    }

    public static IEnumerable<ParameterInfo> GetConstructorParameters(Type type)
    {
        var constructor = GetConstructor(type);

        return constructor != null
            ? constructor.GetParameters()
            : Array.Empty<ParameterInfo>();
    }

    public static IEnumerable<ParameterInfo> GetConstructorParameters(Type type, IEnumerable<Type> parameterTypes)
    {
        var constructor = GetConstructor(type, parameterTypes);

        return constructor != null
            ? constructor.GetParameters()
            : Array.Empty<ParameterInfo>();
    }

    public static FieldInfo[] GetAllFields(Type type)
    {
        var fields = new List<FieldInfo>();

        while (true)
        {
            fields.AddRange(
                type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            var baseType = type.BaseType;
            if (baseType == null)
            {
                break;
            }

            type = baseType;
        }

        return fields.ToArray();
    }

    public static FieldInfo GetBackingField(Type type, string parameterName)
    {
        Guard.Against.NullOrWhitespace(parameterName);

        while (true)
        {
            var field = type.GetField($"<{parameterName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field;
            }

            var baseType = type.BaseType;
            if (baseType == null)
            {
                break;
            }

            type = baseType;
        }

        throw new DependencyException(string.Format(BackingFieldNotFoundMessage, parameterName, type));
    }
}