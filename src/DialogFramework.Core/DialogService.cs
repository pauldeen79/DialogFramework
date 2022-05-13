namespace DialogFramework.Core;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;

    public DialogService(IDialogContextFactory contextFactory)
        => _contextFactory = contextFactory;

    public bool CanStart(IDialog dialog)
        => _contextFactory.Create(dialog).CanStart();

    public IDialogContext Start(IDialog dialog)
    {
        if (!_contextFactory.CanCreate(dialog))
        {
            throw new InvalidOperationException("Could not create context");
        }
        var context = _contextFactory.Create(dialog);
        if (!context.CanStart())
        {
            throw new InvalidOperationException("Could not start dialog");
        }
        try
        {
            var firstPart = dialog.GetFirstPart(context);

            if (firstPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialog);
            }

            while (true)
            {
                if (firstPart is INavigationDialogPart navigationDialogPart)
                {
                    firstPart = navigationDialogPart.GetNextPart(context).ProcessDecisions(context);
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
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public bool CanContinue(IDialogContext context)
        => context.CurrentState == DialogState.InProgress;

    public IDialogContext Continue(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        try
        {
            if (!CanContinue(context))
            {
                throw new InvalidOperationException($"Can only continue when the dialog is in progress. Current state is {context.CurrentState}");
            }

            context = context.AddDialogPartResults(dialogPartResults);
            var nextPart = context.CurrentDialog.GetNextPart(context, context.CurrentPart, dialogPartResults);

            if (nextPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialog);
            }

            while (true)
            {
                if (nextPart is INavigationDialogPart navigationDialogPart)
                {
                    nextPart = navigationDialogPart.GetNextPart(context).ProcessDecisions(context);
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
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public bool CanAbort(IDialogContext context)
        => context.CurrentState == DialogState.InProgress
        && context.CurrentPart.Id != context.CurrentDialog.AbortedPart.Id;

    public IDialogContext Abort(IDialogContext context)
    {
        try
        {
            if (!CanAbort(context))
            {
                throw new InvalidOperationException("Dialog cannot be aborted");
            }

            return context.Abort(context.CurrentDialog.AbortedPart);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public bool CanNavigateTo(IDialogContext context, IDialogPart navigateToPart)
        => (context.CurrentState == DialogState.InProgress || context.CurrentState == DialogState.Completed)
        && context.CanNavigateTo(navigateToPart);

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
        try
        {
            if (!CanResetCurrentState(context))
            {
                throw new InvalidOperationException("Current state cannot be reset");
            }

            return context.ResetDialogPartResultByPart(context.CurrentPart);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }
}
