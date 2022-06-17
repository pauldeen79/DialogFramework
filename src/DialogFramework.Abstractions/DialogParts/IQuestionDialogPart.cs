namespace DialogFramework.Abstractions.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    IReadOnlyCollection<IDialogPartResultDefinition> Results { get; }
    IReadOnlyCollection<IQuestionDialogPartValidator> Validators { get; }
    Result Validate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResult> results);
}
