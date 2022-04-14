namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialog currentDialog)
        : base(currentDialog)
    {
        Answers = new List<IProvidedAnswer>();
    }

    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                DialogState state)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>();
    }

    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                DialogState state,
                                Exception exception)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>();
        Exception = exception;
    }

    private DialogContextFixture(IDialog currentDialog,
                                 IDialogPart currentPart,
                                 DialogState state,
                                 Exception? exception,
                                 IEnumerable<IProvidedAnswer> answers)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>(answers);
        Exception = exception;
    }

    public List<IProvidedAnswer> Answers { get; }

    public Exception? Exception { get; }

    public override IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, DialogState.Aborted);

    public override IDialogContext ProvideAnswers(IEnumerable<IProvidedAnswer> providedAnswers)
        => new DialogContextFixture(CurrentDialog, CurrentPart, CurrentState, null, Answers.Concat(providedAnswers));

    public override IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContextFixture(CurrentDialog, nextPart, state, null, Answers);

    public override IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, DialogState.ErrorOccured, ex);

    public override IDialogContext Start(IDialogPart firstPart)
        => new DialogContextFixture(CurrentDialog, firstPart, firstPart.State);

    public DialogContextFixture WithState(DialogState state)
        => new DialogContextFixture(CurrentDialog, CurrentPart, state, Exception, Answers);
}
