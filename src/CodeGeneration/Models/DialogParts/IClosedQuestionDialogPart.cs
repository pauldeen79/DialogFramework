namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface IClosedQuestionDialogPart : IDialogPart
{
    [Required]
    string Id { get; }
    Evaluatable? Condition { get; }
    [Required]
    string Caption { get; }
    [Required]
    IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
