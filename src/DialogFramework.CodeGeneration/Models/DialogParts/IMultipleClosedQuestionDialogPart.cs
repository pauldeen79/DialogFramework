namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface IMultipleClosedQuestionDialogPart : IDialogPart, IEditableQuestionDialogPart
{
    [Required][ValidateObject] IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
