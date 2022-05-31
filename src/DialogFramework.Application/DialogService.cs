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
        try
        {
            var firstPart = dialog.GetFirstPart(context, _conditionEvaluator);
            return context.Chain(x => x.Start(dialog, firstPart.Id));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Start)} failed";
            _logger.LogError(ex, msg);
            return context.Chain(x => x.Error(dialog, new Error(msg)));
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
            var nextPart = dialog.GetNextPart(context, _conditionEvaluator, dialogPartResults);
            return context.Chain(x => x.Continue(dialog, dialogPartResults, nextPart.Id, nextPart.GetValidationResults()));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Continue)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Chain(x => x.Error(dialog, new Error(msg)));
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
            return context.Chain(x => x.Abort(dialog));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(Abort)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Chain(x => x.Error(dialog, new Error(msg)));
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
            return context.Chain(x => x.NavigateTo(dialog, navigateToPartId));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(NavigateTo)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Chain(x => x.Error(dialog, new Error(msg)));
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
            return context.Chain(x => x.ResetCurrentState(dialog));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(ResetCurrentState)} failed";
            _logger.LogError(ex, msg);
            if (dialog != null)
            {
                return context.Chain(x => x.Error(dialog, new Error(msg)));
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
