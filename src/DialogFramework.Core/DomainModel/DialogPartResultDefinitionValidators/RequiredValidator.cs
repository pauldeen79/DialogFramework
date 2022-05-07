namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitionValidators;

public class RequiredValidator : IDialogPartResultDefinitionValidator
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext,
                                                  IDialogPart dialogPart,
                                                  IDialogPartResultDefinition dialogPartResultDefinition,
                                                  IEnumerable<IDialogPartResult> dialogPartResults)
    {
        if (!dialogPartResults.Any()
            || dialogPartResults.Any(x => x.Value.Value == null || x.Value.Value is string s && string.IsNullOrEmpty(s)))
        {
            yield return new ValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is required");
        }
    }
}
