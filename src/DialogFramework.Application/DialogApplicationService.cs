namespace DialogFramework.Application;

public class DialogApplicationService : IDialogApplicationService
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IDialogDefinitionProvider _dialogDefinitionProvider;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger _logger;

    public DialogApplicationService(IDialogFactory dialogFactory,
                                    IDialogDefinitionProvider dialogDefinitionProvider,
                                    IConditionEvaluator conditionEvaluator,
                                    ILogger logger)
    {
        _dialogFactory = dialogFactory;
        _dialogDefinitionProvider = dialogDefinitionProvider;
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    public Result<IDialog> Start(IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                                 IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var dialogResult = CreateDialogAndDefinition(dialogDefinitionIdentifier, dialogPartResults);
        if (!dialogResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogResult);
        }
        var dialog = dialogResult.Value!.dialog;
        var definition = dialogResult.Value!.definition;
        return PerformAction(dialog, nameof(Start), _ => dialog.Start(definition, _conditionEvaluator), definition);
    }

    public Result<IDialog> Continue(IDialog dialog,
                                    IEnumerable<IDialogPartResultAnswer> dialogPartResults)
        => PerformAction(dialog, nameof(Continue), dialogDefinition => dialog.Continue(dialogDefinition, dialogPartResults, _conditionEvaluator));

    public Result<IDialog> Abort(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.Abort(dialogDefinition));

    public Result<IDialog> NavigateTo(IDialog dialog,
                                      IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                                      IDialogPartIdentifier navigateToPartId)
        => PerformAction(dialog, nameof(NavigateTo), dialogDefinition => dialog.NavigateTo(dialogDefinition, navigateToPartId), GetDialogDefinition(dialogDefinitionIdentifier).Value);

    public Result<IDialog> ResetCurrentState(IDialog dialog,
                                             IDialogDefinitionIdentifier dialogDefinitionIdentifier)
        => PerformAction(dialog, nameof(ResetCurrentState), dialogDefinition => dialog.ResetCurrentState(dialogDefinition), GetDialogDefinition(dialogDefinitionIdentifier).Value);

    private Result<IDialog> PerformAction(IDialog dialog,
                                          string operationName,
                                          Func<IDialogDefinition, Result> action)
    {
        var dialogDefinitionResult = GetDialogDefinition(dialog.CurrentDialogIdentifier);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogDefinitionResult);
        }
        var definition = dialogDefinitionResult.Value!;
        return PerformAction(dialog, operationName, action, definition);
    }

    private Result<IDialog> PerformAction(IDialog dialog,
                                          string operationName,
                                          Func<IDialogDefinition, Result> action,
                                          IDialogDefinition? definition)
    {
        if (definition == null)
        {
            return PerformAction(dialog, operationName, action);
        }

        try
        {
            var result = action.Invoke(definition);
            if (result.Status == ResultStatus.Redirect
                && result is Result<IDialogDefinitionIdentifier> dialogDefinitionIdentifierResult)
            {
                return Start(dialogDefinitionIdentifierResult.GetValueOrThrow(), dialog.GetAllResults(definition));
            }

            if (!result.IsSuccessful())
            {
                return Result<IDialog>.FromExistingResult(result);
            }

            return Result<IDialog>.Success(dialog);
        }
        catch (Exception ex)
        {
            var msg = $"{operationName} failed";
            _logger.LogError(ex, msg);
            dialog.Error(definition!, new Error(msg));
            return Result<IDialog>.Success(dialog);
        }
    }

    private Result<IDialogDefinition> GetDialogDefinition(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        Result<IDialogDefinition>? definitionResult = null;
        try
        {
            definitionResult = _dialogDefinitionProvider.GetDialogDefinition(dialogDefinitionIdentifier);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(GetDialogDefinition)} failed";
            _logger.LogError(ex, msg);
            return Result<IDialogDefinition>.Error("Could not retrieve dialog definition");
        }

        if (definitionResult.Status == ResultStatus.NotFound)
        {
            var msg = $"Unknown dialog definition: Id [{dialogDefinitionIdentifier.Id}], Version [{dialogDefinitionIdentifier.Version}]";
            _logger.LogError(msg);
            return Result<IDialogDefinition>.NotFound(msg);
        }
        
        return definitionResult;
    }

    private Result<(IDialog dialog, IDialogDefinition definition)> CreateDialogAndDefinition(
        IDialogDefinitionIdentifier dialogDefinitionIdentifier,
        IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var dialogDefinitionResult = GetDialogDefinition(dialogDefinitionIdentifier);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.FromExistingResult(dialogDefinitionResult);
        }

        var dialogDefinition = dialogDefinitionResult.Value!;
        if (!_dialogFactory.CanCreate(dialogDefinition, dialogPartResults))
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.Error("Could not create dialog");
        }

        try
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.Success((_dialogFactory.Create(dialogDefinition, dialogPartResults), dialogDefinition));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            return Result<(IDialog dialog, IDialogDefinition definition)>.Error("Dialog creation failed");
        }
    }
}
