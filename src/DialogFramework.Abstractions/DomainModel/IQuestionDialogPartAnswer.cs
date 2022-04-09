namespace DialogFramework.Abstractions.DomainModel;

public interface IQuestionDialogPartAnswer : IDataErrorInfo
{
    string Id { get; }
    string Title { get; }
    AnswerValueType ValueType { get; }
}
