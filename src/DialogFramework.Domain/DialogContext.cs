namespace DialogFramework.Domain;

public partial record DialogContext
{
    public bool CanAbort(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialog.AbortedPart.Id);

    public void Abort(IDialog dialog)
    {
        CurrentPartId = dialog.AbortedPart.Id;
        CurrentGroupId = dialog.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
    }

    public void AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults)
    {
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialog.ReplaceAnswers(Results, partResults));
    }

    public bool CanContinue(IDialog dialog)
        => CurrentState == DialogState.InProgress;

    public void Continue(IDialog dialog,
                         IDialogPartIdentifier nextPartId,
                         IEnumerable<IDialogValidationResult> validationResults)
    {
        var nextPart = dialog.GetPartById(nextPartId);
        CurrentPartId = nextPartId;
        CurrentGroupId = nextPart.GetGroupId();
        CurrentState = nextPart.GetState();
        ValidationErrors = new ValueCollection<IDialogValidationResult>(validationResults);
    }

    public void Error(IDialog dialog, IEnumerable<IError> errors)
    {
        CurrentPartId = dialog.ErrorPart.Id;
        CurrentGroupId = dialog.ErrorPart.GetGroupId();
        CurrentState = DialogState.ErrorOccured;
        Errors = new ReadOnlyValueCollection<IError>(errors);
    }

    public bool CanStart(IDialog dialog)
        => CurrentState == DialogState.Initial
        && dialog.Metadata.CanStart;

    public void Start(IDialog dialog, IDialogPartIdentifier firstPartId)
    {
        var firstPart = dialog.GetPartById(firstPartId);
        CurrentPartId = firstPartId;
        CurrentGroupId = firstPart.GetGroupId();
        CurrentState = firstPart.GetState();
    }

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => (CurrentState == DialogState.InProgress || CurrentState == DialogState.Completed)
        && dialog.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

    public void NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
    {
        var navigateToPart = dialog.GetPartById(navigateToPartId);
        CurrentPartId = navigateToPartId;
        CurrentGroupId = navigateToPart.GetGroupId();
        CurrentState = navigateToPart.GetState();
    }

    public bool CanResetCurrentState(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && dialog.CanResetPartResultsByPartId(CurrentPartId);

    public void ResetCurrentState(IDialog dialog)
    {
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialog.ResetPartResultsByPartId(Results, CurrentPartId));
    }
}
