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
        Answers = new List<IProvidedAnswer>();
        Exception = exception;
    }

    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                IDialogPartGroup? currentGroup,
                                DialogState state,
                                Exception? exception,
                                IEnumerable<IProvidedAnswer> answers)
        : base(currentDialog, currentPart, currentGroup, state)
    {
        Answers = new List<IProvidedAnswer>(answers);
        Exception = exception;
    }

    public List<IProvidedAnswer> Answers { get; }

    public Exception? Exception { get; }

    public override IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, null, DialogState.Aborted, null);

    public override IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state)
        => new DialogContextFixture(CurrentDialog, nextPart, nextGroup, state, null, Answers.Concat(providedAnswers));

    public override IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, null, DialogState.ErrorOccured, ex);

    public override IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup)
        => new DialogContextFixture(CurrentDialog, firstPart, firstGroup, DialogService.GetState(firstPart), null);

    public DialogContextFixture WithState(DialogState state)
        => new DialogContextFixture(CurrentDialog, CurrentPart, CurrentGroup, state, Exception, Answers);
}
