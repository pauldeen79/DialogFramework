namespace DialogFramework.Domain;

public partial record DialogContext
{
    public bool CanAbort(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialog.AbortedPart.Id);

    public IDialogContext Abort(IDialog dialog)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            dialog.AbortedPart.Id,
            dialog.AbortedPart.GetGroupId(),
            DialogState.Aborted,
            Results,
            Enumerable.Empty<IDialogValidationResult>()
        );

    public IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            CurrentPartId,
            CurrentGroupId,
            CurrentState,
            dialog.ReplaceAnswers(Results, partResults),
            Enumerable.Empty<IDialogValidationResult>()
        );

    public bool CanContinue(IDialog dialog)
        => CurrentState == DialogState.InProgress;

    public IDialogContext Continue(IDialog dialog,
                                   IDialogPartIdentifier nextPartId,
                                   IEnumerable<IDialogValidationResult> validationResults)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            nextPartId,
            dialog.GetPartById(nextPartId).GetGroupId(),
            dialog.GetPartById(nextPartId).GetState(),
            Results,
            validationResults
        );

    public IDialogContext Error(IDialog dialog, string? errorMessage)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            dialog.ErrorPart.Id,
            dialog.ErrorPart.GetGroupId(),
            DialogState.ErrorOccured,
            Results,
            string.IsNullOrEmpty(errorMessage)
                ? Array.Empty<IDialogValidationResult>()
                : new[] { new DialogValidationResultBuilder().WithErrorMessage(errorMessage!).Build() }
        );

    public bool CanStart(IDialog dialog)
        => CurrentState == DialogState.Initial
        && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialog dialog, IDialogPartIdentifier firstPartId)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            firstPartId,
            dialog.GetPartById(firstPartId).GetGroupId(),
            dialog.GetPartById(firstPartId).GetState(),
            Enumerable.Empty<IDialogPartResult>(),
            Enumerable.Empty<IDialogValidationResult>()
        );

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => (CurrentState == DialogState.InProgress || CurrentState == DialogState.Completed)
        && dialog.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

    public IDialogContext NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            navigateToPartId,
            dialog.GetPartById(navigateToPartId).GetGroupId(),
            dialog.GetPartById(navigateToPartId).GetState(),
            Results,
            Enumerable.Empty<IDialogValidationResult>()
        );

    public bool CanResetCurrentState(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && dialog.CanResetPartResultsByPartId(CurrentPartId);

    public IDialogContext ResetCurrentState(IDialog dialog)
        => new DialogContext
        (
            Id,
            CurrentDialogIdentifier,
            CurrentPartId,
            CurrentGroupId,
            CurrentState,
            dialog.ResetPartResultsByPartId(Results, CurrentPartId),
            Enumerable.Empty<IDialogValidationResult>()
        );
}
