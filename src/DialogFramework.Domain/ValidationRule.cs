namespace DialogFramework.Domain;

public partial class ValidationRule
{
    public abstract Result Validate<T>(string id, T value, Dialog dialog);
}
