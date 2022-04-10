namespace DialogFramework.Core.Tests.Fixtures;

internal record QuestionDialogPartFixture : QuestionDialogPart
{
    private readonly Func<IEnumerable<IProvidedAnswer>, string?> _errorMessageDelegate;

    public QuestionDialogPartFixture(string id,
                                     string title,
                                     IDialogPartGroup group,
                                     IEnumerable<IQuestionDialogPartAnswer> answers,
                                     Func<IEnumerable<IProvidedAnswer>, string?> errorMessageDelegate)
        : base(id, title, group, answers)
        => _errorMessageDelegate = errorMessageDelegate;

    public override IDialogPart? Validate(IEnumerable<IProvidedAnswer> providedAnswers)
    {
        var errorMessage = _errorMessageDelegate(providedAnswers);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ErrorMessage = errorMessage;
            return this;
        }

        return null;
    }
}
