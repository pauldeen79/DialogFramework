namespace DialogFramework.Abstractions.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    IReadOnlyCollection<IDialogPartResultDefinition> Results { get; }
    IReadOnlyCollection<IQuestionDialogPartValidator> Validators { get; }
    IDialogPart? Validate(IDialog dialog,
                          IDialogDefinition dialogDefinition,
                          IEnumerable<IDialogPartResult> dialogPartResults);
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
}
