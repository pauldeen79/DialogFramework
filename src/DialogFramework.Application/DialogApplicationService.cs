namespace DialogFramework.Application;

public class DialogApplicationService : IDialogApplicationService
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IDialogDefinitionRepository _dialogDefinitionRepository;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger _logger;

    public DialogApplicationService(IDialogFactory dialogFactory,
                                    IDialogDefinitionRepository dialogDefinitionRepository,
                                    IConditionEvaluator conditionEvaluator,
                                    ILogger logger)
    {
        _dialogFactory = dialogFactory;
        _dialogDefinitionRepository = dialogDefinitionRepository;
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    public Result<IDialog> Start(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        var dialogResult = CreateDialogAndDefinition(dialogDefinitionIdentifier);
        if (!dialogResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogResult);
        }
        var dialog = dialogResult.Value!.dialog;
        var definition = dialogResult.Value!.definition;
        return PerformAction(dialog, nameof(Start), _ => dialog.Start(definition, _conditionEvaluator), definition);
    }

    public Result<IDialog> Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        => PerformAction(dialog, nameof(Continue), dialogDefinition => dialog.Continue(dialogDefinition, dialogPartResults, _conditionEvaluator));

    public Result<IDialog> Abort(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.Abort(dialogDefinition));

    public Result<IDialog> NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => PerformAction(dialog, nameof(NavigateTo), dialogDefinition => dialog.NavigateTo(dialogDefinition, navigateToPartId));

    public Result<IDialog> ResetCurrentState(IDialog dialog)
        => PerformAction(dialog, nameof(ResetCurrentState), dialogDefinition => dialog.ResetCurrentState(dialogDefinition));

    private Result<IDialog> PerformAction(IDialog dialog,
                                          string operationName,
                                          Func<IDialogDefinition, Result> action,
                                          IDialogDefinition? definition = null)
    {
        try
        {
            if (definition == null)
            {
                var dialogDefinitionResult = GetDialogDefinition(dialog.CurrentDialogIdentifier);
                if (!dialogDefinitionResult.IsSuccessful())
                {
                    return Result<IDialog>.FromExistingResult(dialogDefinitionResult);
                }
                definition = dialogDefinitionResult.Value!;
            }
            var result = action.Invoke(definition!);
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
        IDialogDefinition? dialog = null;
        try
        {
            dialog = _dialogDefinitionRepository.GetDialogDefinition(dialogDefinitionIdentifier);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(GetDialogDefinition)} failed";
            _logger.LogError(ex, msg);
            return Result<IDialogDefinition>.Error("Could not retrieve dialog definition");
        }
        if (dialog == null)
        {
            var msg = $"Unknown dialog definition: Id [{dialogDefinitionIdentifier.Id}], Version [{dialogDefinitionIdentifier.Version}]";
            _logger.LogError(msg);
            return Result<IDialogDefinition>.NotFound(msg);
        }
        return Result<IDialogDefinition>.Success(dialog);
    }

    private Result<(IDialog dialog, IDialogDefinition definition)> CreateDialogAndDefinition(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        var dialogDefinitionResult = GetDialogDefinition(dialogDefinitionIdentifier);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.FromExistingResult(dialogDefinitionResult);
        }
        var dialogDefinition = dialogDefinitionResult.Value!;
        if (!_dialogFactory.CanCreate(dialogDefinition))
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.Error("Could not create dialog");
        }
        try
        {
            return Result<(IDialog dialog, IDialogDefinition definition)>.Success((_dialogFactory.Create(dialogDefinition), dialogDefinition));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            return Result<(IDialog dialog, IDialogDefinition definition)>.Error("Dialog creation failed");
        }
    }
}
