namespace DialogFramework.Core;

public class DialogContext : IDialogContext
{
    public DialogContext(IDialog currentDialog)
        : this(Guid.NewGuid().ToString(), currentDialog, new EmptyDialogPart(), DialogState.Initial)
    {
    }

    protected DialogContext(string id,
                            IDialog currentDialog,
                            IDialogPart currentPart,
                            DialogState currentState)
    {
        Answers = new List<IDialogPartResult>();
        Id = id;
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentState = currentState;
        CurrentGroup = currentPart is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
    }

    protected DialogContext(string id,
                            IDialog currentDialog,
                            IDialogPart currentPart,
                            DialogState currentState,
                            Exception? exception,
                            IEnumerable<IDialogPartResult> answers)
        : this(id, currentDialog, currentPart, currentState)
    {
        Exception = exception;
        Answers.AddRange(answers);
    }

    public string Id { get; }
    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; }
    protected List<IDialogPartResult> Answers { get; }
    public Exception? Exception { get; }

    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(Id, CurrentDialog, abortDialogPart, DialogState.Aborted);

    public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults)
        => new DialogContext(Id, CurrentDialog, CurrentPart, CurrentState, null, CurrentDialog.ReplaceAnswers(Answers, dialogPartResults));

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(Id, CurrentDialog, nextPart, state, null, Answers);

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContext(Id, CurrentDialog, errorDialogPart, DialogState.ErrorOccured, ex, Answers);

    public bool CanStart()
       => CurrentDialog.Metadata.CanStart;

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialog, firstPart, firstPart.State);

    public bool CanNavigateTo(IDialogPart navigateToPart)
        => CurrentPart.Id == navigateToPart.Id || Answers.Any(x => x.DialogPart.Id == navigateToPart.Id);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialog, navigateToPart, CurrentState, null, Answers);

    public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
        => Answers.FindAll(x => x.DialogPart.Id == dialogPart.Id);

    public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart)
        => new DialogContext(Id, CurrentDialog, CurrentPart, CurrentState, Exception, CurrentDialog.ResetDialogPartResultByPart(Answers, CurrentPart));

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState State => DialogState.Initial;
    }
}
