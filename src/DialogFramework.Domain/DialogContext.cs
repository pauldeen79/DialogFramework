namespace DialogFramework.Domain;

public partial record DialogContext
{
    public bool CanAbort(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && CurrentPart.Id != dialog.AbortedPart.Id;

    public IDialogContext Abort(IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, dialog.AbortedPart, dialog.AbortedPart.GetGroup(), DialogState.Aborted, Results);

    public IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, dialog.ReplaceAnswers(Results, partResults));

    public bool CanContinue(IDialog dialog)
        => CurrentState == DialogState.InProgress;

    public IDialogContext Continue(IDialog dialog, IDialogPart nextPart)
        => new DialogContext(Id, CurrentDialogIdentifier, nextPart, nextPart.GetGroup(), nextPart.GetState(), Results);

    public IDialogContext Error(IDialog dialog, Exception? exception)
        => new DialogContext(Id, CurrentDialogIdentifier, dialog.ErrorPart, dialog.ErrorPart.GetGroup(), DialogState.ErrorOccured, Results);

    public bool CanStart(IDialog dialog)
        => CurrentState == DialogState.Initial
        && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialog dialog, IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialogIdentifier, firstPart, firstPart.GetGroup(), firstPart.GetState(), Enumerable.Empty<IDialogPartResult>());

    public bool CanNavigateTo(IDialog dialog, IDialogPart navigateToPart)
        => (CurrentState == DialogState.InProgress || CurrentState == DialogState.Completed)
        && dialog.CanNavigateTo(CurrentPart, navigateToPart, Results);

    public IDialogContext NavigateTo(IDialog dialog, IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialogIdentifier, navigateToPart, navigateToPart.GetGroup(), navigateToPart.GetState(), Results);

    public bool CanResetCurrentState(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && CurrentPart is IQuestionDialogPart;

    public IDialogContext ResetCurrentState(IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, dialog.ResetDialogPartResultByPart(Results, CurrentPart));
}
