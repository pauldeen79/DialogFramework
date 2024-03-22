namespace DialogFramework.CodeGeneration.Models.DialogParts.BaseTypes;

public interface IEditableQuestionDialogPart
{
    [Required][ValidateObject] IReadOnlyCollection<IValidationRule> ValidationRules { get; }
}
