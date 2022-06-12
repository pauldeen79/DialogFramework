namespace DialogFramework.Domain;

public partial record Dialog
{
    public Result Abort(IDialogDefinition dialogDefinition)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Error(ResultStatus.Invalid, "Current state is invalid");
        }
        if (Equals(CurrentPartId, dialogDefinition.AbortedPart.Id))
        {
            // Already on the aborted part
            return Result.Error(ResultStatus.Invalid, "Current part is the aborted part");
        }

        CurrentPartId = dialogDefinition.AbortedPart.Id;
        CurrentGroupId = dialogDefinition.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
        return Result.Success();
    }

    public Result Continue(IDialogDefinition dialogDefinition,
                           IEnumerable<IDialogPartResult> partResults,
                           IConditionEvaluator conditionEvaluator)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Error(ResultStatus.Invalid, "Current state is invalid");
        }

        var nextPartResult = dialogDefinition.GetNextPart(this, conditionEvaluator, partResults);
        if (!nextPartResult.IsSuccessful())
        {
            return nextPartResult;
        }
        var nextPart = nextPartResult.Value!;
        CurrentPartId = nextPart.Id;
        CurrentGroupId = nextPart.GetGroupId();
        CurrentState = nextPart.GetState();
        Results = new ReadOnlyValueCollection<IDialogPartResult>(dialogDefinition.ReplaceAnswers(Results, partResults));
        ValidationErrors = new ReadOnlyValueCollection<IDialogValidationResult>(nextPart.GetValidationResults());
        return Result.Success();
    }

    public Result Error(IDialogDefinition dialogDefinition, IEnumerable<IError> errors)
    {
        CurrentPartId = dialogDefinition.ErrorPart.Id;
        CurrentGroupId = dialogDefinition.ErrorPart.GetGroupId();
        CurrentState = dialogDefinition.ErrorPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
        Errors = new ReadOnlyValueCollection<IError>(errors);
        return Result.Success();
    }

    public Result Start(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator)
    {
        if (CurrentState != DialogState.Initial)
        {
            // Wrong state
            return Result.Error(ResultStatus.Invalid, "Current state is invalid");
        }

        if (!dialogDefinition.Metadata.CanStart)
        {
            // Dialog definition cannot be started (only exixting ones can be finished)
            return Result.Error(ResultStatus.Error, "Dialog definition cannot be started");
        }

        var firstPartResult = dialogDefinition.GetFirstPart(this, conditionEvaluator);
        if (!firstPartResult.IsSuccessful())
        {
            return Result.Error(firstPartResult.Status, firstPartResult.ErrorMessage!);
        }
        CurrentPartId = firstPartResult.Value!.Id;
        CurrentGroupId = firstPartResult.Value!.GetGroupId();
        CurrentState = firstPartResult.Value!.GetState();
        return Result.Success();
    }

    public Result NavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Error(ResultStatus.Invalid, "Current state is invalid");
        }

        var canNavigateToResult = dialogDefinition.CanNavigateTo(CurrentPartId, navigateToPartId, Results);
        if(!canNavigateToResult.IsSuccessful())
        {
            // Not possible to navigate to the requested part
            return canNavigateToResult;
        }

        var navigateToPartResult = dialogDefinition.GetPartById(navigateToPartId);
        if (!navigateToPartResult.IsSuccessful())
        {
            return navigateToPartResult;
        }
        var navigateToPart = navigateToPartResult.Value!;
        CurrentPartId = navigateToPartId;
        CurrentGroupId = navigateToPart.GetGroupId();
        CurrentState = navigateToPart.GetState();
        ValidationErrors = new ReadOnlyValueCollection<DialogValidationResult>();
        return Result.Success();
    }

    public Result ResetCurrentState(IDialogDefinition dialogDefinition)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Error(ResultStatus.Invalid, "Current state is invalid");
        }

        var canResetResult = dialogDefinition.ResetPartResultsByPartId(Results, CurrentPartId);
        if (!canResetResult.IsSuccessful())
        {
            return canResetResult;
        }
        Results = new ReadOnlyValueCollection<IDialogPartResult>(canResetResult.Value!);
        ValidationErrors = new ReadOnlyValueCollection<IDialogValidationResult>();
        return Result.Success();
    }
}
