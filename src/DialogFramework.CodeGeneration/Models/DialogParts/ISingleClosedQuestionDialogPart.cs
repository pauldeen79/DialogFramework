namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface ISingleClosedQuestionDialogPart : IDialogPart, IEditableQuestionDialogPart
{
    [Required][ValidateObject] IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
