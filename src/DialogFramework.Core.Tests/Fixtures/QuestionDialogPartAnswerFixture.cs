namespace DialogFramework.Core.Tests.Fixtures;

internal class QuestionDialogPartAnswerFixture : QuestionDialogPartAnswer
{
    private readonly Func<string, string> _indexerErrorMessageDelegate;
    private readonly Func<string> _errorDelegate;

    public QuestionDialogPartAnswerFixture(string id,
                                           string title,
                                           AnswerValueType valueType,
                                           Func<string, string> indexerErrorMessageDelegate,
                                           Func<string> errorDelegate)
        : base(id, title, valueType)
    {
        _indexerErrorMessageDelegate = indexerErrorMessageDelegate;
        _errorDelegate = errorDelegate;
    }

    public override string this[string columnName] => _indexerErrorMessageDelegate(columnName);
    public override string Error => _errorDelegate();
}
