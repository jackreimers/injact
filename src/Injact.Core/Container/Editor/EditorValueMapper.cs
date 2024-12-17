namespace Injact.Core.Container.Editor;

//TODO: Confirm this works when class/interface is assignable
public class EditorValueMapper
{
    private const string MismatchedMappingMessage = "{0} has values marked for mapping but the native class {1} does not have any matching values.";
    private const string MismatchedMappingCountMessage = "{0} has {1} values marked for mapping but only {2} were mapped to the native class {3}.";

    private readonly ILogger _logger;

    public EditorValueMapper(ILogger logger)
    {
        _logger = logger;
    }

    public void Map(object engineObject, object nativeObject)
    {
        var engineType = engineObject.GetType();
        var nativeType = nativeObject.GetType();

        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        var engineTypeFields = engineType
            .GetFields(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();

        var engineTypeProperties = engineType
            .GetProperties(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();

        if (engineTypeFields.Length == 0 && engineTypeProperties.Length == 0)
        {
            return;
        }

        var nativeTypeFields = nativeType
            .GetFields(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();

        var nativeTypeProperties = nativeType
            .GetProperties(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();

        if (nativeTypeFields.Length == 0 && nativeTypeProperties.Length == 0)
        {
            _logger.LogWarning(string.Format(MismatchedMappingMessage, engineType.Name, nativeType));
            return;
        }

        var mappedCount = 0;

        foreach (var property in engineTypeFields)
        {
            var mappedProperty = nativeTypeFields.FirstOrDefault(s => s.Name == property.Name);
            if (mappedProperty == null)
            {
                continue;
            }

            //TODO: Check type is assignable

            mappedProperty.SetValue(nativeObject, property.GetValue(engineObject));
            mappedCount++;
        }

        foreach (var property in engineTypeProperties)
        {
            var mappedProperty = nativeTypeProperties.FirstOrDefault(s => s.Name == property.Name);
            if (mappedProperty == null)
            {
                continue;
            }

            mappedProperty.SetValue(nativeObject, property.GetValue(engineObject));
            mappedCount++;
        }

        if (mappedCount < nativeTypeProperties.Length + nativeTypeProperties.Length)
        {
            _logger.LogWarning(string.Format(
                MismatchedMappingCountMessage,
                engineType.Name,
                nativeTypeProperties.Length + nativeTypeProperties.Length,
                mappedCount,
                nativeType));
        }
    }
}