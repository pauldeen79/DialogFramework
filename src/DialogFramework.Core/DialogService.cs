namespace DialogFramework.Core;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;
    private readonly IDialogRepository _dialogRepository;

    public DialogService(IDialogContextFactory contextFactory, IDialogRepository dialogRepository)
    {
        _contextFactory = contextFactory;
        _dialogRepository = dialogRepository;
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
            var firstPart = dialog.GetFirstPart(context, _dialogRepository);
            if (firstPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialogMetadata);
            }

            while (true)
            {
                if (firstPart is INavigationDialogPart navigationDialogPart)
                {
                    firstPart = dialog.GetPartById(navigationDialogPart.GetNextPartId(context))
                                      .ProcessDecisions(context, _dialogRepository);
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
            var nextPart = dialog.GetNextPart(context, context.CurrentPart, _dialogRepository, dialogPartResults);

            if (nextPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialogMetadata);
            }

            while (true)
            {
                if (nextPart is INavigationDialogPart navigationDialogPart)
                {
                    nextPart = dialog.GetPartById(navigationDialogPart.GetNextPartId(context))
                                     .ProcessDecisions(context, _dialogRepository);
                }
                else
                {
                    break;
                }
            }

            return context.Continue(nextPart, nextPart.State);
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
}
