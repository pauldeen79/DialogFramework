namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, IEnumerable<KeyValuePair<string, object?>>, IDialogPart> _getNextPartDelegate;

    public DecisionDialogPartFixture(string id,
                                     Func<IDialogContext, IEnumerable<KeyValuePair<string, object?>>, IDialogPart> getNextPartDelegate)
        : base(id)
    {
        _getNextPartDelegate = getNextPartDelegate;
    }

    public override IDialogPart GetNextPart(IDialogContext context, IEnumerable<KeyValuePair<string, object?>> answerValues)
        => _getNextPartDelegate(context, answerValues);
}
