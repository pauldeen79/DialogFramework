namespace DialogFramework.Core.Tests.Fixtures;

internal record QuestionDialogPartFixture : QuestionDialogPart
{
    private readonly Func<IEnumerable<KeyValuePair<string, object?>>, string?> _errorMessageDelegate;

    public QuestionDialogPartFixture(string id,
                                     string title,
                                     IDialogPartGroup group,
                                     IEnumerable<IQuestionDialogPartAnswer> answers,
                                     Func<IEnumerable<KeyValuePair<string, object?>>, string?> errorMessageDelegate)
        : base(id, title, group, answers)
    {
        _errorMessageDelegate = errorMessageDelegate;
    }

    public override IDialogPart? Validate(IEnumerable<KeyValuePair<string, object?>> answerValues)
    {
        var errorMessage = _errorMessageDelegate(answerValues);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ErrorMessage = errorMessage;
            return this;
        }
        return null;
    }
}
