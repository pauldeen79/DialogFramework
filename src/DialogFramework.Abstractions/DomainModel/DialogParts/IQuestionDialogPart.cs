namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    ValueCollection<IDialogPartResultDefinition> Results { get; }
    IDialogPart? Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    ValueCollection<IDialogValidationResult> ValidationErrors { get; }
}
