namespace DialogFramework.Core.DomainModel;

public class QuestionDialogPartAnswer : IQuestionDialogPartAnswer
{
    private readonly Func<string, string> _indexerErrorMessageDelegate;
    private readonly Func<string> _errorDelegate;

    public QuestionDialogPartAnswer(string id,
                                    string title,
                                    AnswerValueType valueType,
                                    Func<string, string> indexerErrorMessageDelegate,
                                    Func<string> errorDelegate)
    {
        Id = id;
        Title = title;
        ValueType = valueType;
        _indexerErrorMessageDelegate = indexerErrorMessageDelegate;
        _errorDelegate = errorDelegate;
    }

    public string this[string columnName] => _indexerErrorMessageDelegate(columnName);
    public string Id { get; }
    public string Title { get; }
    public AnswerValueType ValueType { get; }
    public string Error => _errorDelegate();
}
