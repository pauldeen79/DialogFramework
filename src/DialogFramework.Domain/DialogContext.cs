namespace DialogFramework.Domain;

public partial record DialogContext
{
    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(Id, CurrentDialogIdentifier, abortDialogPart, abortDialogPart.GetGroup(), DialogState.Aborted, Results);

    public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, dialog.ReplaceAnswers(Results, dialogPartResults));

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(Id, CurrentDialogIdentifier, nextPart, nextPart.GetGroup(), state, Results);

    public IDialogContext Error(IErrorDialogPart errorDialogPart)
        => new DialogContext(Id, CurrentDialogIdentifier, errorDialogPart, errorDialogPart.GetGroup(), DialogState.ErrorOccured, Results);

    public bool CanStart(IDialog dialog)
       => CurrentState == DialogState.Initial && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialogIdentifier, firstPart, firstPart.GetGroup(), firstPart.GetState(), Enumerable.Empty<IDialogPartResult>());

    public bool CanNavigateTo(IDialogPart navigateToPart, IDialog dialog)
        => dialog.CanNavigateTo(CurrentPart, navigateToPart, Results);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialogIdentifier, navigateToPart, navigateToPart.GetGroup(), navigateToPart.GetState(), Results);

    public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
        => Results.Where(x => x.DialogPartId == dialogPart.Id);

    public IEnumerable<IDialogPartResult> GetAllDialogPartResults() => Results;

    public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, dialog.ResetDialogPartResultByPart(Results, CurrentPart));
}
