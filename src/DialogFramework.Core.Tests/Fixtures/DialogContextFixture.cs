namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                IDialogPartGroup? currentGroup,
                                DialogState state,
                                Exception? exception)
        : base(currentDialog, currentPart, currentGroup, state)
    {
        Exception = exception;
    }

    public List<IProvidedAnswer> Answers { get; }
        = new List<IProvidedAnswer>();

    public Exception? Exception { get; }

    public override IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, null, DialogState.Aborted, null);

    public override IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state)
    {
        Answers.AddRange(providedAnswers);
        return new DialogContextFixture(CurrentDialog, nextPart, nextGroup, state, null);
    }

    public override IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, null, DialogState.ErrorOccured, ex);

    public override IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup)
        => new DialogContextFixture(CurrentDialog, firstPart, firstGroup, DialogState.InProgress, null);
}
