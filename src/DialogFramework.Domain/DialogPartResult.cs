namespace DialogFramework.Domain;

public partial class DialogPartResult
{
    public abstract Result<object?> GetValue();
}
