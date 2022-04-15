namespace DialogFramework.Abstractions.DomainModel;

public interface IQuestionDialogPartResult
{
    string Id { get; }
    string Title { get; }
    AnswerValueType ValueType { get; }
}
