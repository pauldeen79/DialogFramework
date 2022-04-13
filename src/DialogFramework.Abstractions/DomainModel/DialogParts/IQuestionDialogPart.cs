namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Message { get; }
    ValueCollection<IQuestionDialogPartAnswer> Answers { get; }
    IDialogPart? Validate(IEnumerable<IProvidedAnswer> providedAnswers);
}
