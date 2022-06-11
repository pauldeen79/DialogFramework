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
        return PerformAction(dialog, nameof(Start), dialogDefinition => dialog.Start(dialogDefinition, _conditionEvaluator), dialogDefinition);
    }

    public bool CanContinue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        => dialog.CanContinue(GetDialogDefinition(dialog), dialogPartResults);

    public IDialog Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        => PerformAction(dialog, nameof(Continue), dialogDefinition => dialog.Continue(dialogDefinition, dialogPartResults, _conditionEvaluator));

    public bool CanAbort(IDialog dialog)
        => dialog.CanAbort(GetDialogDefinition(dialog));

    public IDialog Abort(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.Abort(dialogDefinition));

    public bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => dialog.CanNavigateTo(GetDialogDefinition(dialog), navigateToPartId);

    public IDialog NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.NavigateTo(dialogDefinition, navigateToPartId));

    public bool CanResetCurrentState(IDialog dialog)
        => dialog.CanResetCurrentState(GetDialogDefinition(dialog));

    public IDialog ResetCurrentState(IDialog dialog)
        => PerformAction(dialog, nameof(Abort), dialogDefinition => dialog.ResetCurrentState(dialogDefinition));

    private IDialog PerformAction(IDialog dialog, string operationName, Action<IDialogDefinition> action, IDialogDefinition? dialogDefinition = null)
    {
        try
        {
            dialogDefinition??= GetDialogDefinition(dialog);
            action.Invoke(dialogDefinition!);
            return dialog;
        }
        catch (Exception ex)
        {
            var msg = $"{operationName} failed";
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
