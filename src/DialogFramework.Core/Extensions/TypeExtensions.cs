namespace DialogFramework.Core.Extensions;

public static class TypeExtensions
{
    /// <summary>
    /// Gets the default value.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns></returns>
    public static object? GetDefaultValue(this Type instance)
        => instance.IsValueType && Nullable.GetUnderlyingType(instance) == null
            ? Activator.CreateInstance(instance)
            : null;
}
