namespace DialogFramework.Core;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;

    public DialogService(IDialogContextFactory contextFactory)
        => _contextFactory = contextFactory;

    public IDialogContext Start(IDialog dialog)
    {
        var context = _contextFactory.Create(dialog);
        try
        {
            var firstPart = dialog.GetNextPart(context, null, Enumerable.Empty<IProvidedAnswer>());

            return firstPart is IRedirectDialogPart redirectDialogPart
                ? Start(redirectDialogPart.RedirectDialog)
                : context.Start(firstPart);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public IDialogContext Continue(IDialogContext context, IEnumerable<IProvidedAnswer> providedAnswers)
    {
        try
        {
            if (context.CurrentState != DialogState.InProgress)
            {
                throw new InvalidOperationException($"Can only continue when the dialog is in progress. Current state is {context.CurrentState}");
            }

            var nextPart = context.CurrentDialog.GetNextPart(context, context.CurrentPart, providedAnswers);

            return nextPart is IRedirectDialogPart redirectDialogPart
                ? Start(redirectDialogPart.RedirectDialog)
                : context.Continue(providedAnswers, nextPart, nextPart.State);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

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
}
