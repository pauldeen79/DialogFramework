namespace DialogFramework.Core.DomainModel;

public abstract record QuestionDialogPartAnswer : IQuestionDialogPartAnswer
{
    protected QuestionDialogPartAnswer(string id,
                                       string title,
                                       AnswerValueType valueType)
    {
        Id = id;
        Title = title;
        ValueType = valueType;
    }

    public abstract string this[string columnName] { get; }
    public string Id { get; }
    public string Title { get; }
    public AnswerValueType ValueType { get; }
    public abstract string Error { get; }
}
