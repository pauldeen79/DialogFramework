﻿namespace DialogFramework.Core;

public class DialogService : IDialogService
{
    private readonly IDialogContextFactory _contextFactory;

    public DialogService(IDialogContextFactory contextFactory)
        => _contextFactory = contextFactory;

    public IDialogContext Abort(IDialogContext context)
    {
        try
        {
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

    public IDialogContext Continue(IDialogContext context, IEnumerable<KeyValuePair<string, object?>> answers)
    {
        try
        {
            if (context.CurrentState != DialogState.InProgress)
            {
                throw new InvalidOperationException($"Can only continue when the dialog is in progress. Current state is {context.CurrentState}");
            }

            var nextPart = GetNextPart(context.CurrentDialog, context, context.CurrentPart, answers);
            var nextGroup = GetGroup(nextPart);
            var state = GetState(nextPart);

            return context.Continue(answers, nextPart, nextGroup, state);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    public IDialogContext Start(IDialog dialog)
    {
        var context = _contextFactory.Create(dialog);
        try
        {
            var firstPart = GetNextPart(dialog, context, null, Enumerable.Empty<KeyValuePair<string, object?>>());
            var firstGroup = GetGroup(firstPart);

            return context.Start(firstPart, firstGroup);
        }
        catch (Exception ex)
        {
            return context.Error(context.CurrentDialog.ErrorPart.ForException(ex), ex);
        }
    }

    private static IDialogPart ProcessDecisions(IDialogPart dialogPart, IDialogContext context)
    {
        if (dialogPart is IDecisionDialogPart decisionDialogPart)
        {
            if (!string.IsNullOrEmpty(decisionDialogPart.Error))
            {
                return new ErrorDialogPart(decisionDialogPart.Id, decisionDialogPart.Error);
            }

            return ProcessDecisions(decisionDialogPart.GetNextPart(context), context);
        }

        return dialogPart;
    }

    private static DialogState GetState(IDialogPart nextPart)
    {
        if (nextPart is IAbortedDialogPart)
        {
            return DialogState.Aborted;
        }

        if (nextPart is ICompletedDialogPart)
        {
            return DialogState.Completed;
        }

        if (nextPart is IDecisionDialogPart)
        {
            throw new InvalidOperationException("Decision dialog part is being returned to the user, this is not allowed!");
        }

        if (nextPart is IErrorDialogPart)
        {
            return DialogState.ErrorOccured;
        }

        if (nextPart is IMessageDialogPart || nextPart is IQuestionDialogPart)
        {
            return DialogState.InProgress;
        }

        throw new InvalidOperationException($"Could not determine dialog state. Next part is of type: {nextPart.GetType()}");
    }

    private static IDialogPart GetNextPart(IDialog dialog,
                                           IDialogContext context,
                                           IDialogPart? currentPart,
                                           IEnumerable<KeyValuePair<string, object?>> answerValues)
    {
        if (currentPart == null)
        {
            // get the first part
            var firstPart = dialog.Parts.FirstOrDefault();
            if (firstPart == null)
            {
                throw new InvalidOperationException("Could not determine next part. Dialog does not have any parts.");
            }

            return ProcessDecisions(firstPart, context);
        }

        // first perform validation
        var error = Validate(currentPart, answerValues);
        if (error != null)
        {
            return error;
        }

        // get the next part
        var parts = dialog.Parts.Select((part, index) => new { Index = index, Part = part }).ToArray();
        var currentPartWithIndex = parts.SingleOrDefault(p => p.Part.Id == currentPart!.Id);
        var nextPartWithIndex = parts.Where(p => p.Index > currentPartWithIndex.Index).OrderBy(p => p.Index).FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            throw new InvalidOperationException($"Could not determine next part. Dialog does not have next part, based on current step (Id = {currentPart.Id})");
        }

        return ProcessDecisions(nextPartWithIndex.Part, context);
    }

    private static IDialogPart? Validate(IDialogPart currentPart, IEnumerable<KeyValuePair<string, object?>> answerValues)
    {
        if (currentPart is IQuestionDialogPart questionDialogPart)
        {
            return questionDialogPart.Validate(answerValues);
        }

        return null;
    }

    private static IDialogPartGroup? GetGroup(IDialogPart? part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
}
