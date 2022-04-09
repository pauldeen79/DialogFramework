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

    public List<KeyValuePair<IDialogPart, IEnumerable<KeyValuePair<string, object?>>>> Answers { get; }
        = new List<KeyValuePair<IDialogPart, IEnumerable<KeyValuePair<string, object?>>>>();

    public Exception? Exception { get; }

    public override IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, null, DialogState.Aborted, null);

    public override IDialogContext Continue(IEnumerable<KeyValuePair<string, object?>> answers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state)
    {
        Answers.Add(new KeyValuePair<IDialogPart, IEnumerable<KeyValuePair<string, object?>>>(CurrentPart, answers));
        return new DialogContextFixture(CurrentDialog, nextPart, nextGroup, state, null);
    }

    public override IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, null, DialogState.ErrorOccured, ex);

    public override IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup)
        => new DialogContextFixture(CurrentDialog, firstPart, firstGroup, DialogState.InProgress, null);
}
