namespace DialogFramework.Domain;

public partial record Dialog
{
    public bool CanAbort(IDialogDefinition dialogDefinition)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }
        if (Equals(CurrentPartId, dialogDefinition.AbortedPart.Id))
        {
            // Already on the aborted part
            return false;
        }

        return true;
    }

    public void Abort(IDialogDefinition dialogDefinition)
    {
        if (!CanAbort(dialogDefinition))
        {
            throw new InvalidOperationException("Dialog cannot be aborted");
        }
        CurrentPartId = dialogDefinition.AbortedPart.Id;
        CurrentGroupId = dialogDefinition.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
    }

    public bool CanContinue(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> partResults)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }

        return true;
    }

    public void Continue(IDialogDefinition dialogDefinition,
                         IEnumerable<IDialogPartResult> partResults,
                         IConditionEvaluator conditionEvaluator)
    {
        if (!CanContinue(dialogDefinition, partResults))
        {
            throw new InvalidOperationException("Can only continue when the dialog is in progress");
        }
        var nextPart = dialogDefinition.GetNextPart(this, conditionEvaluator, partResults);
        CurrentPartId = nextPart.Id;
        CurrentGroupId = nextPart.GetGroupId();
        CurrentState = nextPart.GetState();
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialogDefinition.ReplaceAnswers(Results, partResults));
        ValidationErrors = new ValueCollection<IDialogValidationResult>(nextPart.GetValidationResults());
    }

    public void Error(IDialogDefinition dialogDefinition, IEnumerable<IError> errors)
    {
        CurrentPartId = dialogDefinition.ErrorPart.Id;
        CurrentGroupId = dialogDefinition.ErrorPart.GetGroupId();
        CurrentState = dialogDefinition.ErrorPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
        Errors = new ReadOnlyValueCollection<IError>(errors);
    }

    public bool CanStart(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator)
    {
        if (CurrentState != DialogState.Initial)
        {
            // Wrong state
            return false;
        }

        if (!dialogDefinition.Metadata.CanStart)
        {
            // Dialog definition cannot be started (only exixting ones can be finished)
            return false;
        }

        if (!dialogDefinition.CanGetFirstPart(this, conditionEvaluator))
        {
            // Dialog definition does not have a first part
            return false;
        }

        return true;
    }

    public void Start(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator)
    {
        if (!CanStart(dialogDefinition, conditionEvaluator))
        {
            throw new InvalidOperationException("Could not start dialog");
        }
        var firstPart = dialogDefinition.GetFirstPart(this, conditionEvaluator);
        CurrentPartId = firstPart.Id;
        CurrentGroupId = firstPart.GetGroupId();
        CurrentState = firstPart.GetState();
    }

    public bool CanNavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }

        if (!dialogDefinition.CanNavigateTo(CurrentPartId, navigateToPartId, Results))
        {
            // Not possible to navigate to the requested part
            return false;
        }

        return true;
    }

    public void NavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId)
    {
        if (!CanNavigateTo(dialogDefinition, navigateToPartId))
        {
            throw new InvalidOperationException("Cannot navigate to requested dialog part");
        }
        var navigateToPart = dialogDefinition.GetPartById(navigateToPartId);
        CurrentPartId = navigateToPartId;
        CurrentGroupId = navigateToPart.GetGroupId();
        CurrentState = navigateToPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
    }

    public bool CanResetCurrentState(IDialogDefinition dialogDefinition)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return false;
        }

        if (!dialogDefinition.CanResetPartResultsByPartId(CurrentPartId))
        {
            // Current part does not support reset (possibly not a question?)
            return false;
        }

        return true;
    }

    public void ResetCurrentState(IDialogDefinition dialogDefinition)
    {
        if (!CanResetCurrentState(dialogDefinition))
        {
            throw new InvalidOperationException("Current state cannot be reset");
        }
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialogDefinition.ResetPartResultsByPartId(Results, CurrentPartId));
        ValidationErrors = new ReadOnlyValueCollection<IDialogValidationResult>();
    }
}
