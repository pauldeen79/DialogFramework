namespace DialogFramework.Abstractions.DomainModel;

public interface IQuestionDialogPartAnswer
{
    string Id { get; }
    string Title { get; }
    AnswerValueType ValueType { get; }
}
