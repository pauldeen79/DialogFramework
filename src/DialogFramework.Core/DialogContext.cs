namespace DialogFramework.Core;

public abstract class DialogContext : IDialogContext
{
    protected DialogContext(IDialog currentDialog)
        : this(currentDialog, new EmptyDialogPart(), null, DialogState.Initial)
    {
    }

    protected DialogContext(IDialog currentDialog,
                            IDialogPart currentPart,
                            IDialogPartGroup? currentGroup,
                            DialogState currentState)
    {
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentGroup = currentGroup;
        CurrentState = currentState;
    }

    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; }

    public abstract IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup);
    public abstract IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state);
    public abstract IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    public abstract IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public string Heading => "Empty";
        public DialogState State => DialogState.Initial;
    }
}
