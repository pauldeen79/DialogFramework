namespace DialogFramework.SpecFlow.Tests.Support;

public static class TableValueHelpers
{
    /// <summary>
    /// Replaces values speified from a table that need conversion, for example because the target type is object.
    /// </summary>
    /// <remarks>You can use expressions like [null], [boolean:true], [boolean:false] and [today[</remarks>
    /// <param name="value">input value (automatically mapped table value)</param>
    /// <returns>Corrected value</returns>
    public static object? EvaluateExpressions(object? value)
    {
        if (value is string s)
        {
            switch (s.ToLowerInvariant())
            {
                case "[null]":
                    return null;
                case "[boolean:true]":
                    return true;
                case "[boolean:false]":
                    return false;
                case "[today]":
                    return DateTime.Today;
            }
        }

        return value;
    }
}
