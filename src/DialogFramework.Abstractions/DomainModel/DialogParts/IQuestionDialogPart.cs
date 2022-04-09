namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IQuestionDialogPart : IDialogPart
{
    string Title { get; }
    IDialogPartGroup Group { get; }
    ValueCollection<IQuestionDialogPartAnswer> Answers { get; }
    IDialogPart? Validate(IEnumerable<KeyValuePair<string, object?>> answerValues);
}
