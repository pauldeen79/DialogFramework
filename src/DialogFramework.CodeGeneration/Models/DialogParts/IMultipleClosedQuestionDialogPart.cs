namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface IMultipleClosedQuestionDialogPart : IDialogPart, IEditableQuestionDialogPart
{
    [Required]
    IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
