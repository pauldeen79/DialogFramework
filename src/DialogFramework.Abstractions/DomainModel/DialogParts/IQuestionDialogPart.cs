namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    ValueCollection<IQuestionDialogPartAnswer> Answers { get; }
    IDialogPart? Validate(IEnumerable<KeyValuePair<string, object?>> answerValues);
}
