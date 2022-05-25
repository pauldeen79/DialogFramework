namespace DialogFramework.Core;

public partial record DialogContext
{
    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(Id, CurrentDialogIdentifier, abortDialogPart, (abortDialogPart as IGroupedDialogPart)?.Group, DialogState.Aborted, Answers, null);

    public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, (CurrentPart as IGroupedDialogPart)?.Group, CurrentState, new ValueCollection<IDialogPartResult>(dialog.ReplaceAnswers(Answers, dialogPartResults)), null);

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(Id, CurrentDialogIdentifier, nextPart, (nextPart as IGroupedDialogPart)?.Group, state, new ValueCollection<IDialogPartResult>(Answers), null);

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContext(Id, CurrentDialogIdentifier, errorDialogPart, (errorDialogPart as IGroupedDialogPart)?.Group, DialogState.ErrorOccured, Answers, ex);

    public bool CanStart(IDialog dialog)
       => CurrentState == DialogState.Initial && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(Id, CurrentDialogIdentifier, firstPart, (firstPart as IGroupedDialogPart)?.Group, firstPart.GetState(), new ValueCollection<IDialogPartResult>(), null);

    public bool CanNavigateTo(IDialogPart navigateToPart, IDialog dialog)
        => dialog.CanNavigateTo(CurrentPart, navigateToPart, Answers);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(Id, CurrentDialogIdentifier, navigateToPart, (navigateToPart as IGroupedDialogPart)?.Group, navigateToPart.GetState(), Answers, null);

    public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
        => Answers.Where(x => x.DialogPartId == dialogPart.Id);

    public IEnumerable<IDialogPartResult> GetAllDialogPartResults() => Answers;

    public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart, IDialog dialog)
        => new DialogContext(Id, CurrentDialogIdentifier, CurrentPart, (CurrentPart as IGroupedDialogPart)?.Group, CurrentState, new ValueCollection<IDialogPartResult>(dialog.ResetDialogPartResultByPart(Answers, CurrentPart)), Exception);
}
