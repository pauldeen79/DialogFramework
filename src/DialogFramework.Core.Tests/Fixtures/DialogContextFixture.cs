namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialog currentDialog)
        : base(currentDialog)
    {
        Answers = new List<IProvidedAnswer>();
    }

    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                DialogState state)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>();
    }

    public DialogContextFixture(IDialog currentDialog,
                                IDialogPart currentPart,
                                DialogState state,
                                Exception exception)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>();
        Exception = exception;
    }

    private DialogContextFixture(IDialog currentDialog,
                                 IDialogPart currentPart,
                                 DialogState state,
                                 Exception? exception,
                                 IEnumerable<IProvidedAnswer> answers)
        : base(currentDialog, currentPart, state)
    {
        Answers = new List<IProvidedAnswer>(answers);
        Exception = exception;
    }

    public List<IProvidedAnswer> Answers { get; }

    public Exception? Exception { get; }

    public override IDialogContext Abort(IAbortedDialogPart abortDialogPart)
        => new DialogContextFixture(CurrentDialog, abortDialogPart, DialogState.Aborted);

    public override IDialogContext ProvideAnswers(IEnumerable<IProvidedAnswer> providedAnswers)
        => new DialogContextFixture(CurrentDialog, CurrentPart, CurrentState, null, ReplaceAnswers(providedAnswers));

    public override IDialogContext Continue(IDialogPart nextPart, DialogState state)
        => new DialogContextFixture(CurrentDialog, nextPart, state, null, Answers /*FilterAnswersUntilPart(nextPart)*/);

    public override IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
        => new DialogContextFixture(CurrentDialog, errorDialogPart, DialogState.ErrorOccured, ex);

    public override IDialogContext Start(IDialogPart firstPart)
        => new DialogContextFixture(CurrentDialog, firstPart, firstPart.State);

    public DialogContextFixture WithState(DialogState state)
        => new DialogContextFixture(CurrentDialog, CurrentPart, state, Exception, Answers);

    public override IDialogContext NavigateTo(IDialogPart navigateToPart)
        => new DialogContextFixture(CurrentDialog, navigateToPart, CurrentState, null, Answers);

    private IEnumerable<IProvidedAnswer> ReplaceAnswers(IEnumerable<IProvidedAnswer> providedAnswers)
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
            var index = workingCopy.FindIndex(x => x.Question.Id == providedAnswer.Question.Id);
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
}
