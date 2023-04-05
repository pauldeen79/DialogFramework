namespace DialogFramework.Domain.ValidationRules;

public partial record ConditionalRequiredValidationRule
{
    public override Result Validate<T>(string id, T value, Dialog dialog)
    {
        if (Condition == null || !typeof(T).IsEmptyValue(value))
        {
            return Result.Success();
        }

        var result = Condition.Evaluate(dialog);
        if (!result.IsSuccessful())
        {
            return Result.Error(result.ErrorMessage ?? "Condition evaluation failed");
        }

        return !result.Value
            ? Result.Success()
            : Result.Invalid(new[] { new ValidationError($"{id} is required", new[] { id }) });
    }
}

