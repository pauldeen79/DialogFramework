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

    public bool CanStart(IDialog dialog)
        => _contextFactory.Create(dialog).CanStart(dialog);

    public IDialogContext Start(IDialog dialog)
    {
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
                var metadata = redirectDialogPart.RedirectDialogMetadata;
                var redirectDialog = _dialogRepository.GetDialog(metadata);
                if (redirectDialog == null)
                {
                    throw new InvalidOperationException($"Unknown dialog: Id [{metadata.Id}], Version [{metadata.Version}]");
                }
                return Start(redirectDialog);
            }

            while (true)
            {
                if (firstPart is INavigationDialogPart navigationDialogPart)
                {
                    var id = navigationDialogPart.GetNextPartId(context);
                    firstPart = dialog.GetPartById(id).ProcessDecisions(context, _dialogRepository);
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
                var metadata = redirectDialogPart.RedirectDialogMetadata;
                var redirectDialog = _dialogRepository.GetDialog(metadata);
                if (redirectDialog == null)
                {
                    throw new InvalidOperationException($"Unknown dialog: Id [{metadata.Id}], Version [{metadata.Version}]");
                }
                return Start(redirectDialog);
            }

            while (true)
            {
                if (nextPart is INavigationDialogPart navigationDialogPart)
                {
                    var id = navigationDialogPart.GetNextPartId(context);
                    nextPart = dialog.GetPartById(id).ProcessDecisions(context, _dialogRepository);
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
            dialog = _dialogRepository.GetDialog(context.CurrentDialogIdentifier);
            if (dialog == null)
            {
                throw new InvalidOperationException($"Unknown dialog: Id [{context.CurrentDialogIdentifier.Id}], Version [{context.CurrentDialogIdentifier.Version}]");
            }

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
        if (!CanNavigateTo(context, navigateToPart))
        {
            throw new InvalidOperationException("Cannot navigate to requested dialog part");
        }

        return context.NavigateTo(navigateToPart);
    }

    public bool CanResetCurrentState(IDialogContext context)
        => context.CurrentState == DialogState.InProgress
        && context.CurrentPart is IQuestionDialogPart;

    public IDialogContext ResetCurrentState(IDialogContext context)
    {
        IDialog? dialog = null;
        try
        {
            dialog = _dialogRepository.GetDialog(context.CurrentDialogIdentifier);
            if (dialog == null)
            {
                throw new InvalidOperationException($"Unknown dialog: Id [{context.CurrentDialogIdentifier.Id}], Version [{context.CurrentDialogIdentifier.Version}]");
            }

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

    private IDialog GetDialog(IDialogContext context)
    {
        var dialog = _dialogRepository.GetDialog(context.CurrentDialogIdentifier);
        if (dialog == null)
        {
            throw new InvalidOperationException($"Unknown dialog: Id [{context.CurrentDialogIdentifier.Id}], Version [{context.CurrentDialogIdentifier.Version}]");
        }
        return dialog;
    }
}
