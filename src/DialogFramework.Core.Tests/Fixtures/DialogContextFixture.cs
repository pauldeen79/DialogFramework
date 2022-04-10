namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : IDialogContext
{
    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                IDialogPartGroup? currentGroup,
                                DialogState currentState,
                                Exception? exception,
                                IEnumerable<IProvidedAnswer> answers)
    {
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentGroup = currentGroup;
        CurrentState = currentState;
        Exception = exception;
        Answers.AddRange(answers);
    }

    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; private set; }

    public List<IProvidedAnswer> Answers { get; }
        = new List<IProvidedAnswer>();

    public Exception? Exception { get; }

    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, null, DialogState.Aborted, null, Answers);

    public IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state)
        => new DialogContextFixture(CurrentDialog, nextPart, nextGroup, state, null, Answers.Concat(providedAnswers));

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, null, DialogState.ErrorOccured, ex, Answers);

    public IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup)
        => new DialogContextFixture(CurrentDialog, firstPart, firstGroup, DialogService.GetState(firstPart), null, Answers);

    internal void SetState(DialogState currentState)
        => CurrentState = currentState;
}
