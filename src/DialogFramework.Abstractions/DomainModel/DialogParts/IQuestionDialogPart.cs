namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    ValueCollection<IDialogPartResultDefinition> Results { get; }
    ValueCollection<IQuestionDialogPartValidator> Validators { get; }
    IDialogPart? Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    ValueCollection<IDialogValidationResult> ValidationErrors { get; }
}
