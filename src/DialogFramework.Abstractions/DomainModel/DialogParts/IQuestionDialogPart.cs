namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    IReadOnlyCollection<IDialogPartResultDefinition> Results { get; }
    IReadOnlyCollection<IQuestionDialogPartValidator> Validators { get; }
    IDialogPart? Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
}
