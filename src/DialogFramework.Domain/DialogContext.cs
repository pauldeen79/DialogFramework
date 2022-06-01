namespace DialogFramework.Domain;

public partial record DialogContext
{
    public bool CanAbort(IDialog dialog)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialog.AbortedPart.Id);

    public void Abort(IDialog dialog)
    {
        if (!CanAbort(dialog))
        {
            throw new InvalidOperationException("Dialog cannot be aborted");
        }
        CurrentPartId = dialog.AbortedPart.Id;
        CurrentGroupId = dialog.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
    }

    public bool CanContinue(IDialog dialog, IEnumerable<IDialogPartResult> partResults)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }

        //TODO: Validate that part results are results of the current part.

        //TODO: Validate that next part id is either the current part, before the current part, error/aborted part or the next part. can't continue to parts after the one next to the current part.

        return true;
    }

    public void Continue(IDialog dialog,
                         IEnumerable<IDialogPartResult> partResults,
                         IConditionEvaluator conditionEvaluator,
                         IEnumerable<IDialogValidationResult> validationResults)
    {
        if (!CanContinue(dialog, partResults))
        {
            throw new InvalidOperationException("Can only continue when the dialog is in progress");
        }
        var nextPart = dialog.GetNextPart(this, conditionEvaluator, partResults);
        CurrentPartId = nextPart.Id;
        CurrentGroupId = nextPart.GetGroupId();
        CurrentState = nextPart.GetState();
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialog.ReplaceAnswers(Results, partResults));
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
    {
        if (CurrentState != DialogState.Initial)
        {
            return false;
        }

        if (!dialog.Metadata.CanStart)
        {
            return false;
        }

        return true;
    }

    public void Start(IDialog dialog, IConditionEvaluator conditionEvaluator)
    {
        if (!CanStart(dialog))
        {
            throw new InvalidOperationException("Could not start dialog");
        }
        var firstPart = dialog.GetFirstPart(this, conditionEvaluator);
        CurrentPartId = firstPart.Id;
        CurrentGroupId = firstPart.GetGroupId();
        CurrentState = firstPart.GetState();
    }

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => (CurrentState == DialogState.InProgress || CurrentState == DialogState.Completed)
        && dialog.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

    public void NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
    {
        if (!CanNavigateTo(dialog, navigateToPartId))
        {
            throw new InvalidOperationException("Cannot navigate to requested dialog part");
        }
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
        if (!CanResetCurrentState(dialog))
        {
            throw new InvalidOperationException("Current state cannot be reset");
        }
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialog.ResetPartResultsByPartId(Results, CurrentPartId));
    }
}
