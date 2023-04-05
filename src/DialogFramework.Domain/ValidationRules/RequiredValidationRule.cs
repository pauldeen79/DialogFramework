namespace DialogFramework.Domain.ValidationRules;

public partial record RequiredValidationRule
{
    public override Result Validate<T>(string id, T value, Dialog dialog)
    {
        if (!typeof(T).IsEmptyValue(value))
        {
            return Result.Success();
        }

        return Result.Invalid(new[] { new ValidationError($"{id} is required", new[] { id }) });
    }
}

