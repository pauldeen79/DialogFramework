namespace DialogFramework.Domain;

public partial record DialogContext
{
    public bool CanAbort(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialog.AbortedPart.Id);

    public IDialogContext Abort(IDialog dialog)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(dialog.AbortedPart.Id))
            .WithCurrentGroupId(dialog.AbortedPart.GetGroupIdBuilder())
            .WithCurrentState(DialogState.Aborted)
            .AddResults(Results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(CurrentPartId))
            .WithCurrentGroupId(CurrentGroupId == null ? null : new DialogPartGroupIdentifierBuilder(CurrentGroupId))
            .WithCurrentState(CurrentState)
            .AddResults(dialog.ReplaceAnswers(Results, partResults).Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public bool CanContinue(IDialog dialog)
        => CurrentState == DialogState.InProgress;

    public IDialogContext Continue(IDialog dialog,
                                   IDialogPartIdentifier nextPartId,
                                   IEnumerable<IDialogValidationResult> validationResults)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(nextPartId))
            .WithCurrentGroupId(dialog.GetPartById(nextPartId).GetGroupIdBuilder())
            .WithCurrentState(dialog.GetPartById(nextPartId).GetState())
            .AddResults(Results.Select(x => new DialogPartResultBuilder(x)))
            .AddValidationErrors(validationResults.Select(x => new DialogValidationResultBuilder(x)))
            .Build();

    public IDialogContext Error(IDialog dialog, IEnumerable<string> errorMessages)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(dialog.ErrorPart.Id))
            .WithCurrentGroupId(dialog.ErrorPart.GetGroupIdBuilder())
            .WithCurrentState(DialogState.ErrorOccured)
            .AddResults(Results.Select(x => new DialogPartResultBuilder(x)))
            .AddErrors(errorMessages.Select(x => new ErrorBuilder().WithMessage(x)))
            .Build();

    public bool CanStart(IDialog dialog)
        => CurrentState == DialogState.Initial
        && dialog.Metadata.CanStart;

    public IDialogContext Start(IDialog dialog, IDialogPartIdentifier firstPartId)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(firstPartId))
            .WithCurrentGroupId(dialog.GetPartById(firstPartId).GetGroupIdBuilder())
            .WithCurrentState(dialog.GetPartById(firstPartId).GetState())
            .Build();

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => (CurrentState == DialogState.InProgress || CurrentState == DialogState.Completed)
        && dialog.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

    public IDialogContext NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(navigateToPartId))
            .WithCurrentGroupId(dialog.GetPartById(navigateToPartId).GetGroupIdBuilder())
            .WithCurrentState(dialog.GetPartById(navigateToPartId).GetState())
            .AddResults(Results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public bool CanResetCurrentState(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && dialog.CanResetPartResultsByPartId(CurrentPartId);

    public IDialogContext ResetCurrentState(IDialog dialog)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder(Id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(CurrentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(CurrentPartId))
            .WithCurrentGroupId(CurrentGroupId == null ? null : new DialogPartGroupIdentifierBuilder(CurrentGroupId))
            .WithCurrentState(CurrentState)
            .AddResults(dialog.ResetPartResultsByPartId(Results, CurrentPartId).Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
