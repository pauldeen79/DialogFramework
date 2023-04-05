namespace DialogFramework.Domain.Extensions;

public static class TypeExtensions
{
    public static object? GetDefaultValue(this Type instance)
        => instance.IsValueType && Nullable.GetUnderlyingType(instance) == null
            ? Activator.CreateInstance(instance)
            : null;

    public static bool IsEmptyValue<T>(this Type instance, T value)
    {
        if (value is string s)
        {
            return string.IsNullOrEmpty(s);
        }

        return (object?)value == instance.GetDefaultValue();
    }
}
