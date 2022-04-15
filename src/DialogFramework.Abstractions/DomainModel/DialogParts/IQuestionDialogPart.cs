namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Message { get; }
    ValueCollection<IQuestionDialogPartResult> Results { get; }
    IDialogPart? Validate(IEnumerable<IDialogPartResult> dialogPartResults);
    ValueCollection<string> ErrorMessages { get; }
}
