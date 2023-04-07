namespace DialogFramework.Domain;

public partial record DialogPart
{
    protected Result Validate<T>(T value, Dialog dialog, IReadOnlyCollection<ValidationRule> validationRules)
    {
        var validationErrors = new List<ValidationError>();
        foreach (var rule in validationRules)
        {
            var result = rule.Validate(Id, value, dialog);
            if (result.Status == ResultStatus.Invalid)
            {
                validationErrors.AddRange(result.ValidationErrors);
            }
            else if (!result.IsSuccessful())
            {
                // Something went wrong, probably an error?
                return result;
            }
        }

        return validationErrors.Any()
            ? Result.Invalid($"Validation for dialog part with id [{Id}] failed", validationErrors)
            : Result.Success();
    }
}
