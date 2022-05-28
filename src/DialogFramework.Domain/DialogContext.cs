namespace DialogFramework.Domain;

public partial record DialogContext
{
    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(Id, CurrentDialogIdentifier, abortDialogPart, abortDialogPart.GetGroup(), DialogState.Aborted, Answers, null);

    public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, new ReadOnlyValueCollection<IDialogPartResult>(dialog.ReplaceAnswers(Answers, dialogPartResults)), null);

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(Id, CurrentDialogIdentifier, nextPart, nextPart.GetGroup(), state, new ReadOnlyValueCollection<IDialogPartResult>(Answers), null);

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContext(Id, CurrentDialogIdentifier, errorDialogPart, errorDialogPart.GetGroup(), DialogState.ErrorOccured, Answers, ex);

    public bool CanStart(IDialog dialog)
       => CurrentState == DialogState.Initial && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialogIdentifier, firstPart, firstPart.GetGroup(), firstPart.GetState(), new ReadOnlyValueCollection<IDialogPartResult>(), null);

    public bool CanNavigateTo(IDialogPart navigateToPart, IDialog dialog)
        => dialog.CanNavigateTo(CurrentPart, navigateToPart, Answers);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialogIdentifier, navigateToPart, navigateToPart.GetGroup(), navigateToPart.GetState(), Answers, null);

    public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
        => Answers.Where(x => x.DialogPartId == dialogPart.Id);

    public IEnumerable<IDialogPartResult> GetAllDialogPartResults() => Answers;

    public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, CurrentPart.GetGroup(), CurrentState, new ReadOnlyValueCollection<IDialogPartResult>(dialog.ResetDialogPartResultByPart(Answers, CurrentPart)), Exception);
}
