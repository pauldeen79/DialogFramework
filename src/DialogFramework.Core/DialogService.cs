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
            var firstPart = dialog.GetNextPart(context, null, Enumerable.Empty<IDialogPartResult>());

            if (firstPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialog);
            }
            
            if (firstPart is INavigationDialogPart navigationDialogPart)
            {
                firstPart = navigationDialogPart.GetNextPart(context);
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
            if (context.CurrentState != DialogState.InProgress)
            {
                throw new InvalidOperationException($"Can only continue when the dialog is in progress. Current state is {context.CurrentState}");
            }

            context = context.AddDialogPartResults(dialogPartResults);
            var nextPart = context.CurrentDialog.GetNextPart(context, context.CurrentPart, dialogPartResults);

            if (nextPart is IRedirectDialogPart redirectDialogPart)
            {
                return Start(redirectDialogPart.RedirectDialog);
            }

            if (nextPart is INavigationDialogPart navigationDialogPart)
            {
                nextPart = navigationDialogPart.GetNextPart(context);
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
            if (context.CurrentState == DialogState.ErrorOccured || context.CurrentState == DialogState.Completed)
            {
                throw new InvalidOperationException("Dialog cannot be aborted");
            }

            if (context.CurrentState == DialogState.Aborted)
            {
                throw new InvalidOperationException("Dialog has already been aborted");
            }

            var abortDialogPart = context.CurrentDialog.AbortedPart;
            if (context.CurrentPart.Id == abortDialogPart.Id)
            {
                throw new InvalidOperationException("Dialog has already been aborted");
            }

            return context.Abort(abortDialogPart);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public bool CanNavigateTo(IDialogContext context, IDialogPart navigateToPart)
        => context.CanNavigateTo(navigateToPart);

    public IDialogContext NavigateTo(IDialogContext context, IDialogPart navigateToPart)
    {
        if (!CanNavigateTo(context, navigateToPart))
        {
            throw new InvalidOperationException("Cannot navigate to requested dialog part");
        }

        return context.NavigateTo(navigateToPart);
    }
}
