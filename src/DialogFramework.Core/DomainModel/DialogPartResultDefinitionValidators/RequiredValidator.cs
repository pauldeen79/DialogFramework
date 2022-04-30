namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitionValidators;

public class RequiredValidator : IDialogPartResultDefinitionValidator
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult)
    {
        if (dialogPartResult.Value.Value == null || dialogPartResult.Value.Value is string s && string.IsNullOrEmpty(s))
        {
            yield return new ValidationResult($"Result value of [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] is required");
        }
    }
}
