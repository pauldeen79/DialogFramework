namespace DialogFramework.Application;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;
    private readonly IDialogRepository _dialogRepository;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger _logger;

    public DialogService(IDialogContextFactory contextFactory,
                         IDialogRepository dialogRepository,
                         IConditionEvaluator conditionEvaluator,
                         ILogger logger)
    {
        _contextFactory = contextFactory;
        _dialogRepository = dialogRepository;
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    public bool CanStart(IDialogIdentifier dialogIdentifier)
    {
        var dialog = GetDialog(dialogIdentifier);
        return _contextFactory.Create(dialog).CanStart(dialog);
    }

    public IDialogContext Start(IDialogIdentifier dialogIdentifier)
    {
        var dialog = GetDialog(dialogIdentifier);
        if (!_contextFactory.CanCreate(dialog))
        {
            throw new InvalidOperationException("Could not create context");
        }
        var context = _contextFactory.Create(dialog);
        if (!context.CanStart(dialog))
        {
            throw new InvalidOperationException("Could not start dialog");
        }
        try
        {
            var firstPart = dialog.GetFirstPart(context, _conditionEvaluator);
            return context.Start(dialog, firstPart.Id);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            return context.Error(dialog, new Error(msg));
        }
    }

    public bool CanContinue(IDialogContext context)
        => context.CanContinue(GetDialog(context));

    public IDialogContext Continue(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        IDialog? dialog = null;
        try
        {
            dialog = GetDialog(context);
            if (!CanContinue(context))
            {
                throw new InvalidOperationException($"Can only continue when the dialog is in progress. Current state is {context.CurrentState}");
            }

            context = context.AddDialogPartResults(dialog, dialogPartResults);
            var nextPart = dialog.GetNextPart(context, _conditionEvaluator, dialogPartResults);
            return context.Continue(dialog, nextPart.Id, nextPart.GetValidationResults());
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Continue)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Error(dialog, new Error(msg));
            }
            throw;
        }
    }

    public bool CanAbort(IDialogContext context)
        => context.CanAbort(GetDialog(context));

    public IDialogContext Abort(IDialogContext context)
    {
        IDialog? dialog = null;
        try
        {
            dialog = GetDialog(context);
            if (!CanAbort(context))
            {
                throw new InvalidOperationException("Dialog cannot be aborted");
            }

            return context.Abort(dialog);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Abort)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Error(dialog, new Error(msg));
            }
            throw;
        }
    }

    public bool CanNavigateTo(IDialogContext context, IDialogPartIdentifier navigateToPartId)
        => context.CanNavigateTo(GetDialog(context), navigateToPartId);

    public IDialogContext NavigateTo(IDialogContext context, IDialogPartIdentifier navigateToPartId)
    {
        IDialog? dialog = null;
        try
        {
            dialog = GetDialog(context);
            if (!CanNavigateTo(context, navigateToPartId))
            {
                throw new InvalidOperationException("Cannot navigate to requested dialog part");
            }

            return context.NavigateTo(dialog, navigateToPartId);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(NavigateTo)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Error(dialog, new Error(msg));
            }
            throw;
        }
    }

    public bool CanResetCurrentState(IDialogContext context)
        => context.CanResetCurrentState(GetDialog(context));

    public IDialogContext ResetCurrentState(IDialogContext context)
    {
        IDialog? dialog = null;
        try
        {
            dialog = GetDialog(context);
            if (!CanResetCurrentState(context))
            {
                throw new InvalidOperationException("Current state cannot be reset");
            }

            return context.ResetCurrentState(dialog);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(ResetCurrentState)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Error(dialog, new Error(msg));
            }
            throw;
        }
    }

    private IDialog GetDialog(IDialogContext context) => GetDialog(context.CurrentDialogIdentifier);

    private IDialog GetDialog(IDialogIdentifier dialogIdentifier)
    {
        var dialog = _dialogRepository.GetDialog(dialogIdentifier);
        if (dialog == null)
        {
            throw new InvalidOperationException($"Unknown dialog: Id [{dialogIdentifier.Id}], Version [{dialogIdentifier.Version}]");
        }
        return dialog;
    }
}
