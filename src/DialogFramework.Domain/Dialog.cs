namespace DialogFramework.Domain;

public partial record Dialog
{
    public Result Abort(IDialogDefinition definition)
    {
        if (Equals(CurrentPartId, definition.AbortedPart.Id))
        {
            // Already on the aborted part
            return Result.Invalid("Dialog has already been aborted");
        }

        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        CurrentPartId = definition.AbortedPart.Id;
        CurrentGroupId = definition.AbortedPart.GetGroupId();
        CurrentState = DialogState.Aborted;
        return Result.Success();
    }

    public Result Continue(IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> results, IConditionEvaluator evaluator)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        var nextPartResult = definition.GetNextPart(this, evaluator, results);
        if (!nextPartResult.IsSuccessful())
        {
            return nextPartResult;
        }
        var nextPart = nextPartResult.Value!;
        Results = new ReadOnlyValueCollection<IDialogPartResult>(definition.ReplaceAnswers(Results, results, CurrentPartId));
        CurrentPartId = nextPart.Id;
        CurrentGroupId = nextPart.GetGroupId();
        CurrentState = nextPart.GetState();
        return Result.Success();
    }

    public Result Error(IDialogDefinition definition, IError? error)
    {
        CurrentPartId = definition.ErrorPart.Id;
        CurrentGroupId = definition.ErrorPart.GetGroupId();
        CurrentState = definition.ErrorPart.GetState();
        ErrorMessage = error?.Message;
        return Result.Success();
    }

    public Result Start(IDialogDefinition definition, IConditionEvaluator evaluator)
    {
        if (CurrentState != DialogState.Initial)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        if (!definition.Metadata.CanStart)
        {
            // Dialog definition cannot be started (only exixting ones can be finished)
            return Result.Invalid("Dialog definition cannot be started");
        }

        var firstPartResult = definition.GetFirstPart(this, evaluator);
        if (!firstPartResult.IsSuccessful())
        {
            return Result.Error(firstPartResult.ErrorMessage);
        }
        CurrentPartId = firstPartResult.Value!.Id;
        CurrentGroupId = firstPartResult.Value!.GetGroupId();
        CurrentState = firstPartResult.Value!.GetState();
        return Result.Success();
    }

    public Result NavigateTo(IDialogDefinition definition, IDialogPartIdentifier navigateToPartId)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        var canNavigateToResult = definition.CanNavigateTo(CurrentPartId, navigateToPartId, Results);
        if(!canNavigateToResult.IsSuccessful())
        {
            // Not possible to navigate to the requested part
            return canNavigateToResult;
        }

        var navigateToPartResult = definition.GetPartById(navigateToPartId);
        if (!navigateToPartResult.IsSuccessful())
        {
            return navigateToPartResult;
        }
        var navigateToPart = navigateToPartResult.Value!;
        CurrentPartId = navigateToPartId;
        CurrentGroupId = navigateToPart.GetGroupId();
        CurrentState = navigateToPart.GetState();
        return Result.Success();
    }

    public Result ResetCurrentState(IDialogDefinition definition)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        var canResetResult = definition.ResetPartResultsByPartId(Results, CurrentPartId);
        if (!canResetResult.IsSuccessful())
        {
            return canResetResult;
        }
        Results = new ReadOnlyValueCollection<IDialogPartResult>(canResetResult.Value!);
        return Result.Success();
    }
}
