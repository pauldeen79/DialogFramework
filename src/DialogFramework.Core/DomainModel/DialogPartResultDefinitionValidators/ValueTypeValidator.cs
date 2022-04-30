namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitionValidators;

public class ValueTypeValidator : IDialogPartResultDefinitionValidator
{
    public Type Type { get; }

    public ValueTypeValidator(Type type) => Type = type;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult)
    {
        if (dialogPartResult.Value.Value != null && !Type.IsInstanceOfType(dialogPartResult.Value.Value))
        {
            yield return new ValidationResult($"Result value of [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] is not of type [{Type.FullName}]");
        }
    }
}
