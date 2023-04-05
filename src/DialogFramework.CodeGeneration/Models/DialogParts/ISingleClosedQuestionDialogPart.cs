namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface ISingleClosedQuestionDialogPart : IDialogPart, IEditableQuestionDialogPart
{
    [Required]
    IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
