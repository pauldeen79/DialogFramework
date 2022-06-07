namespace DialogFramework.Domain;

public partial record Dialog
{
    public bool CanAbort(IDialogDefinition dialogDefinition)
        => CurrentState == DialogState.InProgress
        && !Equals(CurrentPartId, dialogDefinition.AbortedPart.Id);

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
            return false;
        }

        if (!dialogDefinition.Metadata.CanStart)
        {
            return false;
        }

        if (!dialogDefinition.CanStart(this, conditionEvaluator))
        {
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
        => CurrentState == DialogState.InProgress
        && dialogDefinition.CanNavigateTo(CurrentPartId, navigateToPartId, Results);

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
        => CurrentState == DialogState.InProgress
        && dialogDefinition.CanResetPartResultsByPartId(CurrentPartId);

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
