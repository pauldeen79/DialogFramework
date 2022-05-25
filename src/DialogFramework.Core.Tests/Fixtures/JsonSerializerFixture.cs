namespace DialogFramework.Core.Tests.Fixtures;

internal static class JsonSerializerFixture
{
    internal static string Serialize(object instance)
        => JsonConvert.SerializeObject(instance, CreateSettings());

    internal static T Deserialize<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, CreateSettings());

    private static JsonSerializerSettings CreateSettings()
        => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new[] { new StringEnumConverter() }
        };
}
