﻿namespace DialogFramework.Core;

public class DialogContext : IDialogContext
{
    public DialogContext(IDialog currentDialog)
        : this(currentDialog, new EmptyDialogPart(), DialogState.Initial)
    {
    }

    protected DialogContext(IDialog currentDialog,
                            IDialogPart currentPart,
                            DialogState currentState)
    {
        Answers = new List<IProvidedAnswer>();
        CurrentDialog = currentDialog;
        CurrentPart = currentPart;
        CurrentState = currentState;
        CurrentGroup = currentPart is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
    }

    protected DialogContext(IDialog currentDialog,
                            IDialogPart currentPart,
                            DialogState currentState,
                            Exception? exception,
                            IEnumerable<IProvidedAnswer> answers)
        : this(currentDialog, currentPart, currentState)
    {
        Exception = exception;
        Answers.AddRange(answers);
    }

    public IDialog CurrentDialog { get; }
    public IDialogPart CurrentPart { get; }
    public IDialogPartGroup? CurrentGroup { get; }
    public DialogState CurrentState { get; }
    protected List<IProvidedAnswer> Answers { get; }
    public Exception? Exception { get; }

    public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContext(CurrentDialog, abortDialogPart, DialogState.Aborted);

    public IDialogContext ProvideAnswers(IEnumerable<IProvidedAnswer> providedAnswers)
        => new DialogContext(CurrentDialog, CurrentPart, CurrentState, null, ReplaceAnswers(providedAnswers));

    public IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContext(CurrentDialog, nextPart, state, null, Answers);

    public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContext(CurrentDialog, errorDialogPart, DialogState.ErrorOccured, ex, Answers);

    public IDialogContext Start(IDialogPart firstPart)
        => new DialogContext(CurrentDialog, firstPart, firstPart.State);

    public bool CanNavigateTo(IDialogPart navigateToPart)
        => CurrentPart.Id == navigateToPart.Id || Answers.Any(x => x.DialogPart.Id == navigateToPart.Id);

    public IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContext(CurrentDialog, navigateToPart, CurrentState, null, Answers);

    public IProvidedAnswer? GetProvidedAnswerByPart(IDialogPart dialogPart)
        => Answers.Find(x => x.DialogPart.Id == dialogPart.Id);

    public IDialogContext ResetProvidedAnswerByPart(IDialogPart dialogPart)
    {
        var answerIndex = Answers.FindIndex(x => x.DialogPart.Id == dialogPart.Id);
        return answerIndex == -1
            ? this
            : new DialogContext(CurrentDialog, CurrentPart, CurrentState, Exception, Answers.Take(answerIndex));
    }

    protected IEnumerable<IProvidedAnswer> ReplaceAnswers(IEnumerable<IProvidedAnswer> providedAnswers)
    {
        if (!providedAnswers.Any())
        {
            // no current provided answers, so no need to merge
            return Answers.Concat(providedAnswers);
        }

        // possibly need to merge provided answers, in case the user navigated back and re-entered the answers
        var workingCopy = new List<IProvidedAnswer>(Answers);
        foreach (var providedAnswer in providedAnswers)
        {
            var index = workingCopy.FindIndex(x => x.DialogPart.Id == providedAnswer.DialogPart.Id);
            if (index > -1)
            {
                // replace existing answer
                workingCopy[index] = providedAnswer;
            }
            else
            {
                // add new answer
                workingCopy.Add(providedAnswer);
            }
        }

        return workingCopy;
    }

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState State => DialogState.Initial;
    }
}
