namespace DialogFramework.Domain;

public partial record DialogPartResult
{
    public abstract object? GetValue();
}
