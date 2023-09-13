namespace DialogFramework.Domain;

public partial record ValidationRule
{
    public abstract Result Validate<T>(string id, T value, Dialog dialog);
}
