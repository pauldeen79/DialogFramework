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
        var dialogDefinitionResult = GetDialogDefinition(dialogDefinitionIdentifier);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogDefinitionResult);
        }
        var dialogDefinition = dialogDefinitionResult.Value!;
        if (!_dialogFactory.CanCreate(dialogDefinition))
        {
            return Result<IDialog>.Error("Could not create dialog");
        }
        IDialog? dialog = null;
        try
        {
            dialog = _dialogFactory.Create(dialogDefinition);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            return Result<IDialog>.Error("Dialog creation failed");
        }
        return PerformAction(dialog, nameof(Start), dialogDefinition => dialog.Start(dialogDefinition, _conditionEvaluator), dialogDefinition);
    }

    public Result<IDialog> Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        => PerformAction(dialog, nameof(Continue), dialogDefinition => dialog.Continue(dialogDefinition, dialogPartResults, _conditionEvaluator));

    public Result<IDialog> Abort(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.Abort(dialogDefinition));

    public Result<IDialog> NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.NavigateTo(dialogDefinition, navigateToPartId));

    public Result<IDialog> ResetCurrentState(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.ResetCurrentState(dialogDefinition));

    private Result<IDialog> PerformAction(IDialog dialog, string operationName, Func<IDialogDefinition, Result> action, IDialogDefinition? dialogDefinition = null)
    {
        try
        {
            if (dialogDefinition == null)
            {
                var dialogDefinitionResult = GetDialogDefinition(dialog);
                if (!dialogDefinitionResult.IsSuccessful())
                {
                    return Result<IDialog>.FromExistingResult(dialogDefinitionResult);
                }
                dialogDefinition = dialogDefinitionResult.Value!;
            }
            var result = action.Invoke(dialogDefinition!);
            if (!result.IsSuccessful())
            {
                //return Result<IDialog>.FromExistingResult(result);
                dialog.Error(dialogDefinition, new Error(result.ErrorMessage ?? string.Empty));
                return Result<IDialog>.Success(dialog);
            }
            return Result<IDialog>.Success(dialog);
        }
        catch (Exception ex)
        {
            var msg = $"{operationName} failed";
            _logger.LogError(ex, msg);
            if (dialogDefinition != null)
            {
                dialog.Error(dialogDefinition, new Error(msg));
                return Result<IDialog>.Success(dialog);
            }
            throw;
        }
    }

    private Result<IDialogDefinition> GetDialogDefinition(IDialog dialog) => GetDialogDefinition(dialog.CurrentDialogIdentifier);

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
}
