namespace DialogFramework.Domain;

public partial record Dialog
{
    private ValueCollection<IDialogPartResult> _results = new();
    private ValueCollection<IProperty> _properties = new();

    public List<IProperty> AddedProperties { get; } = new();

    public Result Abort(IDialogDefinition definition, IConditionEvaluator evaluator)
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

        return HandleNavigate
        (
            definition,
            evaluator,
            Result<IDialogPart>.Success(definition.AbortedPart),
            DialogAction.Abort,
            partResult =>
            {
                if (partResult.IsSuccessful())
                {
                    CurrentPartId = partResult.Value!.Id;
                    CurrentGroupId = partResult.Value!.GetGroupId();
                    CurrentState = DialogState.Aborted;
                }
            },
            _ => Result.Success()
        );
    }

    public Result Continue(IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> results, IConditionEvaluator evaluator)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        _results = new ValueCollection<IDialogPartResult>(definition.ReplaceAnswers(Results, results, CurrentDialogIdentifier, CurrentPartId));

        return HandleNavigate
        (
            definition,
            evaluator,
            definition.GetNextPart(this, results),
            DialogAction.Continue,
            partResult =>
            {
                if (partResult.IsSuccessful())
                {
                    CurrentPartId = partResult.Value!.Id;
                    CurrentGroupId = partResult.Value!.GetGroupId();
                    CurrentState = partResult.Value!.GetState();
                }
            },
            partResult =>
            {
                if (!partResult.IsSuccessful())
                {
                    return partResult;
                }
                else if (partResult.Value is IRedirectDialogPart redirectDialogPart)
                {
                    return Result<IDialogDefinitionIdentifier>.Redirect(redirectDialogPart.RedirectDialogMetadata);
                }

                return Result.Success();
            });
    }

    public Result Error(IDialogDefinition definition, IConditionEvaluator evaluator, IError? error)
        => HandleNavigate
        (
            definition,
            evaluator,
            Result<IDialogPart>.Success(definition.ErrorPart),
            DialogAction.Error,
            partResult =>
            {
                if (partResult.IsSuccessful())
                {
                    CurrentPartId = partResult.Value!.Id;
                    CurrentGroupId = partResult.Value!.GetGroupId();
                    CurrentState = partResult.Value!.GetState();
                    ErrorMessage = error?.Message;
                }
            },
            _ => Result.Success());

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

        return HandleNavigate
        (
            definition,
            evaluator,
            definition.GetFirstPart(),
            DialogAction.Start,
            partResult =>
            {
                if (partResult.IsSuccessful())
                {
                    CurrentPartId = partResult.Value!.Id;
                    CurrentGroupId = partResult.Value!.GetGroupId();
                    CurrentState = partResult.Value!.GetState();
                }
            },
            partResult =>
            {
                if (partResult.Value is IRedirectDialogPart redirectDialogPart)
                {
                    return Result<IDialogDefinitionIdentifier>.Redirect(redirectDialogPart.RedirectDialogMetadata);
                }

                if (!partResult.IsSuccessful())
                {
                    return partResult;
                }

                return Result.Success();
            });
    }

    public Result NavigateTo(IDialogDefinition definition, IDialogPartIdentifier navigateToPartId, IConditionEvaluator evaluator)
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

        return HandleNavigate
        (
            definition,
            evaluator,
            definition.GetPartById(navigateToPartId),
            DialogAction.NavigateTo,
            partResult =>
            {
                if (partResult.IsSuccessful())
                {
                    CurrentDialogIdentifier = definition.Metadata;
                    CurrentPartId = partResult.Value!.Id;
                    CurrentGroupId = partResult.Value!.GetGroupId();
                    CurrentState = partResult.Value!.GetState();
                }
            },
            partResult =>
            {
                if (!partResult.IsSuccessful())
                {
                    return partResult;
                }

                return Result.Success();
            }
        );
    }

    public Result ResetState(IDialogDefinition definition, IDialogPartIdentifier partId)
    {
        if (CurrentState != DialogState.InProgress)
        {
            // Wrong state
            return Result.Invalid("Current state is invalid");
        }

        var canResetResult = definition.ResetPartResultsByPartId(Results, partId);
        if (!canResetResult.IsSuccessful())
        {
            return canResetResult;
        }

        Results = new ReadOnlyValueCollection<IDialogPartResult>(canResetResult.Value!);

        return Result.Success();
    }

    public Result<IEnumerable<IDialogPartResult>> GetDialogPartResultsByPartIdentifier(IDialogPartIdentifier dialogPartIdentifier)
        => Result<IEnumerable<IDialogPartResult>>.Success(Results.Where(x => Equals(x.DialogPartId, dialogPartIdentifier)));

    public void AddProperty(IProperty property)
        => AddedProperties.Add(property);

    private Result HandleNavigate(
        IDialogDefinition definition,
        IConditionEvaluator evaluator,
        Result<IDialogPart> nextPartResult,
        DialogAction action,
        Action<Result<IDialogPart>> callback,
        Func<Result<IDialogPart>, Result> returnDelegate)
    {
        var afterArgs = new AfterNavigateArguments(this, definition, evaluator, action);
        var previousPart = CurrentState == DialogState.Initial
            ? null
            : definition.GetPartById(CurrentPartId).Value;
        var afterPartResult = previousPart?.AfterNavigate(afterArgs);
        ProcessAddedProperties();
        if (afterPartResult != null)
        {
            UpdateState(afterPartResult);
            return returnDelegate(afterPartResult);
        }

        var beforeArgs = new BeforeNavigateArguments(this, definition, evaluator, action);
        var beforePartResult = nextPartResult.Value?.BeforeNavigate(beforeArgs);
        if (beforePartResult == null)
        {
            callback.Invoke(nextPartResult);
        }
        else
        {
            UpdateState(beforePartResult);
        }

        ProcessAddedProperties();
        if (beforePartResult != null)
        {
            return returnDelegate(beforePartResult);
        }

        return returnDelegate(nextPartResult);
    }

    private void UpdateState(Result<IDialogPart> beforePartResult)
    {
        if (beforePartResult.IsSuccessful())
        {
            var part = beforePartResult.GetValueOrThrow();
            CurrentPartId = part.Id;
            CurrentGroupId = part.GetGroupId();
            CurrentState = part.GetState();
        }
    }

    private void ProcessAddedProperties()
    {
        if (AddedProperties.Any())
        {
            _properties.AddRange(AddedProperties);
            AddedProperties.Clear();
        }
    }
}
