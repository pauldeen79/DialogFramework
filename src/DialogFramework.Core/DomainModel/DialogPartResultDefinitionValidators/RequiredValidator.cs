namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitionValidators;

public class RequiredValidator : IDialogPartResultDefinitionValidator
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult)
    {
        if (dialogPartResult.Value.Value == null)
        {
            yield return new ValidationResult("Value is required");
        }
    }
}
