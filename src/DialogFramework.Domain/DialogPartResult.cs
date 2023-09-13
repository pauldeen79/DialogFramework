namespace DialogFramework.Domain;

public partial record DialogPartResult
{
    public abstract Result<object?> GetValue();
}
