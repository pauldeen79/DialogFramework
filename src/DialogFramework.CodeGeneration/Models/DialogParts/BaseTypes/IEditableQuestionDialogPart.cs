namespace DialogFramework.CodeGeneration.Models.DialogParts.BaseTypes;

public interface IEditableQuestionDialogPart
{
    [Required]
    IReadOnlyCollection<IValidationRule> ValidationRules { get; }
}
