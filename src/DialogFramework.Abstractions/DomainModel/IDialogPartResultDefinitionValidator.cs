namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultDefinitionValidator
{
    IEnumerable<ValidationResult> Validate(ValidationContext validationContext,
                                           IDialogPart dialogPart,
                                           IDialogPartResultDefinition dialogPartResultDefinition,
                                           IEnumerable<IDialogPartResult> dialogPartResults);
}
