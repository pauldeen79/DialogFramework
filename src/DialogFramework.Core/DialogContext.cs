namespace DialogFramework.Core;

public abstract class DialogContext : IDialogContext
{
    protected  DialogContext(IDialog currentDialog,
                             IDialogPart currentPart,
                             IDialogPartGroup? currentGroup,
                             DialogState state)
    {
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentGroup = currentGroup;
        State = state;
    }

    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState State { get; }

    public abstract IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup);
    public abstract IDialogContext Continue(IEnumerable<KeyValuePair<string, object?>> answers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state);
    public abstract IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    public abstract IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
}
