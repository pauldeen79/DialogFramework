namespace DialogFramework.Domain;

public partial record Dialog
{
    public bool CanAbort(IDialogDefinition dialog)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialog.AbortedPart.Id);

    public void Abort(IDialogDefinition dialog)
    {
        if (!CanAbort(dialog))
        {
            throw new InvalidOperationException("Dialog cannot be aborted");
        }
        CurrentPartId = dialog.AbortedPart.Id;
        CurrentGroupId = dialog.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
    }

    public bool CanContinue(IDialogDefinition dialog, IEnumerable<IDialogPartResult> partResults)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }

        return true;
    }

    public void Continue(IDialogDefinition dialog,
                         IEnumerable<IDialogPartResult> partResults,
                         IConditionEvaluator conditionEvaluator)
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
        ValidationErrors = new ValueCollection<IDialogValidationResult>(nextPart.GetValidationResults());
    }

    public void Error(IDialogDefinition dialog, IEnumerable<IError> errors)
    {
        CurrentPartId = dialog.ErrorPart.Id;
        CurrentGroupId = dialog.ErrorPart.GetGroupId();
        CurrentState = dialog.ErrorPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
        Errors = new ReadOnlyValueCollection<IError>(errors);
    }

    public bool CanStart(IDialogDefinition dialog, IConditionEvaluator conditionEvaluator)
    {
        if (CurrentState != DialogState.Initial)
        {
            return false;
        }

        if (!dialog.Metadata.CanStart)
        {
            return false;
        }

        if (!dialog.CanStart(this, conditionEvaluator))
        {
            return false;
        }

        return true;
    }

    public void Start(IDialogDefinition dialog, IConditionEvaluator conditionEvaluator)
    {
        if (!CanStart(dialog, conditionEvaluator))
        {
            throw new InvalidOperationException("Could not start dialog");
        }
        var firstPart = dialog.GetFirstPart(this, conditionEvaluator);
        CurrentPartId = firstPart.Id;
        CurrentGroupId = firstPart.GetGroupId();
        CurrentState = firstPart.GetState();
    }

    public bool CanNavigateTo(IDialogDefinition dialog, IDialogPartIdentifier navigateToPartId)
        => CurrentState == DialogState.InProgress
        && dialog.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

    public void NavigateTo(IDialogDefinition dialog, IDialogPartIdentifier navigateToPartId)
    {
        if (!CanNavigateTo(dialog, navigateToPartId))
        {
            throw new InvalidOperationException("Cannot navigate to requested dialog part");
        }
        var navigateToPart = dialog.GetPartById(navigateToPartId);
        CurrentPartId = navigateToPartId;
        CurrentGroupId = navigateToPart.GetGroupId();
        CurrentState = navigateToPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
    }

    public bool CanResetCurrentState(IDialogDefinition dialog)
        => CurrentState == DialogState.InProgress
        && dialog.CanResetPartResultsByPartId(CurrentPartId);

    public void ResetCurrentState(IDialogDefinition dialog)
    {
        if (!CanResetCurrentState(dialog))
        {
            throw new InvalidOperationException("Current state cannot be reset");
        }
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialog.ResetPartResultsByPartId(Results, CurrentPartId));
        ValidationErrors = new ReadOnlyValueCollection<IDialogValidationResult>();
    }
}
