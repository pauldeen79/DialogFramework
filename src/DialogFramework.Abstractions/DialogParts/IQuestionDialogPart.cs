namespace DialogFramework.Abstractions.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    IReadOnlyCollection<IDialogPartResultDefinition> Results { get; }
    IReadOnlyCollection<IQuestionDialogPartValidator> Validators { get; }
    IDialogPart? Validate(IDialog context,
                          IDialogDefinition dialog,
                          IEnumerable<IDialogPartResult> dialogPartResults);
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
}
