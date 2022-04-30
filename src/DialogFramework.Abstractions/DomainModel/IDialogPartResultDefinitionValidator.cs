namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultDefinitionValidator
{
    IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult);
}
