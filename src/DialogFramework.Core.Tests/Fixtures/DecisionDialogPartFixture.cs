namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, IEnumerable<IProvidedAnswer>, IDialogPart> _getNextPartDelegate;

    public DecisionDialogPartFixture(string id,
                                     Func<IDialogContext, IEnumerable<IProvidedAnswer>, IDialogPart> getNextPartDelegate)
        : base(id) => _getNextPartDelegate = getNextPartDelegate;

    public override IDialogPart GetNextPart(IDialogContext context, IEnumerable<IProvidedAnswer> providedAnswers)
        => _getNextPartDelegate(context, providedAnswers);
}
