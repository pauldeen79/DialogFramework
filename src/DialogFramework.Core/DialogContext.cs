namespace DialogFramework.Core;

public abstract class DialogContext : IDialogContext
{
    protected DialogContext(IDialog currentDialog)
        : this(currentDialog, new EmptyDialogPart(), DialogState.Initial)
    {
    }

    protected DialogContext(IDialog currentDialog,
                            IDialogPart currentPart,
                            DialogState currentState)
    {
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentState = currentState;
        CurrentGroup = currentPart is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
    }

    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; }

    public abstract IDialogContext Start(IDialogPart firstPart);
    public abstract IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, DialogState state);
    public abstract IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    public abstract IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState State => DialogState.Initial;
    }
}
