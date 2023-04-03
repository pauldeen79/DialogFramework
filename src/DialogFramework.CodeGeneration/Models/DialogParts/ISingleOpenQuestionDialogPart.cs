namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface ISingleOpenQuestionDialogPart : IDialogPart
{
    //TODO: Review if we can move this to base interface
    [Required]
    IReadOnlyCollection<IValidationRule> ValidationRules { get; }
}
