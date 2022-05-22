namespace DialogFramework.Core;

public class DialogContext : IDialogContext
{
    public DialogContext(IDialogIdentifier currentDialogIdentifier)
        : this(Guid.NewGuid().ToString(), currentDialogIdentifier, new EmptyDialogPart(), DialogState.Initial)
    {
    }

    protected DialogContext(string id,
                            IDialogIdentifier currentDialogIdentifier,
                            IDialogPart currentPart,
                            DialogState currentState)
    {
        Answers = new List<IDialogPartResult>();
        Id = id;
        CurrentDialogIdentifier = currentDialogIdentifier;
        CurrentPart = currentPart;
        CurrentState = currentState;
        CurrentGroup = currentPart is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
    }

    protected DialogContext(string id,
                            IDialogIdentifier currentDialogIdentifier,
                            IDialogPart currentPart,
                            DialogState currentState,
                            Exception? exception,
                            IEnumerable<IDialogPartResult> answers)
        : this(id, currentDialogIdentifier, currentPart, currentState)
    {
        Exception = exception;
        Answers.AddRange(answers);
    }

    public string Id { get; }
    public IDialogIdentifier CurrentDialogIdentifier { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; }
    protected List<IDialogPartResult> Answers { get; }
    public Exception? Exception { get; }

    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(Id, CurrentDialogIdentifier, abortDialogPart, DialogState.Aborted);

    public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentState, null, dialog.ReplaceAnswers(Answers, dialogPartResults));

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(Id, CurrentDialogIdentifier, nextPart, state, null, Answers);

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContext(Id, CurrentDialogIdentifier, errorDialogPart, DialogState.ErrorOccured, ex, Answers);

    public bool CanStart(IDialog dialog)
       => CurrentState == DialogState.Initial && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialogIdentifier, firstPart, firstPart.State);

    public bool CanNavigateTo(IDialogPart navigateToPart, IDialog dialog)
        => dialog.CanNavigateTo(CurrentPart, navigateToPart, Answers);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialogIdentifier, navigateToPart, navigateToPart.State, null, Answers);

    public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
        => Answers.FindAll(x => x.DialogPartId == dialogPart.Id);

    public IEnumerable<IDialogPartResult> GetAllDialogPartResults() => Answers.AsReadOnly();

    public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentState, Exception, dialog.ResetDialogPartResultByPart(Answers, CurrentPart));

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState State => DialogState.Initial;
    }
}
