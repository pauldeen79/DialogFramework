namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface IMultipleClosedQuestionDialogPart : IDialogPart
{
    //TODO: Review if we can move this to base interface
    [Required]
    IReadOnlyCollection<IValidationRule> ValidationRules { get; }

    [Required]
    IReadOnlyCollection<IClosedQuestionOption> Options { get; }
}
