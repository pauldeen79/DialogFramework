namespace DialogFramework.Application;

public class DialogService : IDialogService
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IDialogDefinitionRepository _dialogDefinitionRepository;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger _logger;

    public DialogService(IDialogFactory dialogFactory,
                         IDialogDefinitionRepository dialogDefinitionRepository,
                         IConditionEvaluator conditionEvaluator,
                         ILogger logger)
    {
        _dialogFactory = dialogFactory;
        _dialogDefinitionRepository = dialogDefinitionRepository;
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    public bool CanStart(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        var dialogDefinition = GetDialogDefinition(dialogDefinitionIdentifier);
        var dialog = _dialogFactory.Create(dialogDefinition);
        
        return dialog.CanStart(dialogDefinition, _conditionEvaluator);
    }

    public IDialog Start(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        var dialogDefinition = GetDialogDefinition(dialogDefinitionIdentifier);
        if (!_dialogFactory.CanCreate(dialogDefinition))
        {
            throw new InvalidOperationException("Could not create dialog");
        }
        var dialog = _dialogFactory.Create(dialogDefinition);
        try
        {
            dialog.Start(dialogDefinition, _conditionEvaluator);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            dialog.Error(dialogDefinition, new Error(msg));
            return dialog;
        }
    }

    public bool CanContinue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        return dialog.CanContinue(GetDialogDefinition(dialog), dialogPartResults);
    }

    public IDialog Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        IDialogDefinition? dialogDefinition = null;
        try
        {
            dialogDefinition = GetDialogDefinition(dialog);
            dialog.Continue(dialogDefinition, dialogPartResults, _conditionEvaluator);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Continue)} failed";
            _logger.LogError(ex, msg);
            if (dialogDefinition != null)
            {
                dialog.Error(dialogDefinition, new Error(msg));
                return dialog;
            }
            throw;
        }
    }

    public bool CanAbort(IDialog dialog)
        => dialog.CanAbort(GetDialogDefinition(dialog));

    public IDialog Abort(IDialog dialog)
    {
        IDialogDefinition? dialogDefinition = null;
        try
        {
            dialogDefinition = GetDialogDefinition(dialog);
            dialog.Abort(dialogDefinition);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Abort)} failed";
            _logger.LogError(ex, msg);
            if (dialogDefinition != null)
            {
                dialog.Error(dialogDefinition, new Error(msg));
                return dialog;
            }
            throw;
        }
    }

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => dialog.CanNavigateTo(GetDialogDefinition(dialog), navigateToPartId);

    public IDialog NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
    {
        IDialogDefinition? dialogDefinition = null;
        try
        {
            dialogDefinition = GetDialogDefinition(dialog);
            dialog.NavigateTo(dialogDefinition, navigateToPartId);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(NavigateTo)} failed";
            _logger.LogError(ex, msg);
            if (dialogDefinition != null)
            {
                dialog.Error(dialogDefinition, new Error(msg));
                return dialog;
            }
            throw;
        }
    }

    public bool CanResetCurrentState(IDialog dialog)
        => dialog.CanResetCurrentState(GetDialogDefinition(dialog));

    public IDialog ResetCurrentState(IDialog dialog)
    {
        IDialogDefinition? dialogDefinition = null;
        try
        {
            dialogDefinition = GetDialogDefinition(dialog);
            dialog.ResetCurrentState(dialogDefinition);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(ResetCurrentState)} failed";
            _logger.LogError(ex, msg);
            if (dialogDefinition != null)
            {
                dialog.Error(dialogDefinition, new Error(msg));
                return dialog;
            }
            throw;
        }
    }

    private IDialogDefinition GetDialogDefinition(IDialog dialog) => GetDialogDefinition(dialog.CurrentDialogIdentifier);

    private IDialogDefinition GetDialogDefinition(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        var dialog = _dialogDefinitionRepository.GetDialogDefinition(dialogDefinitionIdentifier);
        if (dialog == null)
        {
            throw new InvalidOperationException($"Unknown dialog definition: Id [{dialogDefinitionIdentifier.Id}], Version [{dialogDefinitionIdentifier.Version}]");
        }
        return dialog;
    }
}
