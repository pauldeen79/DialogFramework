namespace DialogFramework.Domain;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;
    private readonly IDialogRepository _dialogRepository;
    private readonly IConditionEvaluator _conditionEvaluator;

    public DialogService(IDialogContextFactory contextFactory,
                         IDialogRepository dialogRepository,
                         IConditionEvaluator conditionEvaluator)
    {
        _contextFactory = contextFactory;
        _dialogRepository = dialogRepository;
        _conditionEvaluator = conditionEvaluator;
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
            var firstPart = GetFirstPart(dialog, context);
            if (firstPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialogMetadata);
            }

            while (true)
            {
                if (firstPart is INavigationDialogPart navigationDialogPart)
                {
                    firstPart = ProcessDecisions(dialog.GetPartById(navigationDialogPart.GetNextPartId(context)), context);
                }
                else
                {
                    break;
                }
            }

            return context.Start(firstPart);
        }
        catch (Exception ex)
        {
            return context.Error(dialog.ErrorPart.ForException(ex), ex);
        }
    }

    public bool CanContinue(IDialogContext context)
        => context.CurrentState == DialogState.InProgress;

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

            context = context.AddDialogPartResults(dialogPartResults, dialog);
            var nextPart = GetNextPart(dialog, context, context.CurrentPart, dialogPartResults);

            if (nextPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialogMetadata);
            }

            while (true)
            {
                if (nextPart is INavigationDialogPart navigationDialogPart)
                {
                    nextPart = ProcessDecisions(dialog.GetPartById(navigationDialogPart.GetNextPartId(context)), context);
                }
                else
                {
                    break;
                }
            }

            return context.Continue(nextPart, nextPart.GetState());
        }
        catch (Exception ex)
        {
            if (dialog != null)
            {
                return context.Error(dialog.ErrorPart.ForException(ex), ex);
            }
            throw;
        }
    }

    public bool CanAbort(IDialogContext context)
        => context.CurrentState == DialogState.InProgress
        && context.CurrentPart.Id != GetDialog(context).AbortedPart.Id;

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

            return context.Abort(dialog.AbortedPart);
        }
        catch (Exception ex)
        {
            if (dialog != null)
            {
                return context.Error(dialog.ErrorPart.ForException(ex), ex);
            }
            throw;
        }
    }

    public bool CanNavigateTo(IDialogContext context, IDialogPart navigateToPart)
        => (context.CurrentState == DialogState.InProgress || context.CurrentState == DialogState.Completed)
        && context.CanNavigateTo(navigateToPart, GetDialog(context));

    public IDialogContext NavigateTo(IDialogContext context, IDialogPart navigateToPart)
    {
        IDialog? dialog = null;
        try
        {
            dialog = GetDialog(context);
            if (!CanNavigateTo(context, navigateToPart))
            {
                throw new InvalidOperationException("Cannot navigate to requested dialog part");
            }

            return context.NavigateTo(navigateToPart);
        }
        catch (Exception ex)
        {
            if (dialog != null)
            {
                return context.Error(dialog.ErrorPart.ForException(ex), ex);
            }
            throw;
        }
    }

    public bool CanResetCurrentState(IDialogContext context)
        => context.CurrentState == DialogState.InProgress
        && context.CurrentPart is IQuestionDialogPart;

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

            return context.ResetDialogPartResultByPart(context.CurrentPart, dialog);
        }
        catch (Exception ex)
        {
            if (dialog != null)
            {
                return context.Error(dialog.ErrorPart.ForException(ex), ex);
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

    private IDialogPart GetFirstPart(IDialog dialog, IDialogContext context)
    {
        var firstPart = dialog.Parts.FirstOrDefault();
        if (firstPart == null)
        {
            throw new InvalidOperationException("Could not determine next part. Dialog does not have any parts.");
        }

        return ProcessDecisions(firstPart, context);
    }

    private IDialogPart ProcessDecisions(IDialogPart dialogPart, IDialogContext context)
    {
        if (dialogPart is IDecisionDialogPart decisionDialogPart)
        {
            var dialog = GetDialog(context.CurrentDialogIdentifier);
            if (decisionDialogPart is IConditionEvaluatorContainer evaluatorContainer)
            {
                evaluatorContainer.ConditionEvaluator = _conditionEvaluator;
            }
            var nextPartId = decisionDialogPart.GetNextPartId(context, dialog);
            return ProcessDecisions(dialog.GetPartById(nextPartId), context);
        }

        return dialogPart;
    }

    private IDialogPart GetNextPart(IDialog dialog,
                                    IDialogContext context,
                                    IDialogPart currentPart,
                                    IEnumerable<IDialogPartResult> providedAnswers)
    {
        // first perform validation
        var error = currentPart.Validate(context, dialog, providedAnswers);
        if (error != null)
        {
            return error;
        }

        // if validation succeeds, then get the next part
        var parts = dialog.Parts.Select((part, index) => new { Index = index, Part = part }).ToArray();
        var currentPartWithIndex = parts.SingleOrDefault(p => p.Part.Id == currentPart.Id);
        var nextPartWithIndex = parts.Where(p => currentPartWithIndex != null && p.Index > currentPartWithIndex.Index).OrderBy(p => p.Index).FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return ProcessDecisions(dialog.CompletedPart, context);
        }

        return ProcessDecisions(nextPartWithIndex.Part, context);
    }
}
