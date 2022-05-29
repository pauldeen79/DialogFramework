namespace DialogFramework.Domain.Builders;

public class DialogPartBuilder
{
    public QuestionDialogPartBuilder? QuestionDialogPartBuilder { get; set; }
    public AbortedDialogPartBuilder? AbortedDialogPartBuilder { get; set;  }
    public ErrorDialogPartBuilder? ErrorDialogPartBuilder { get; set;  }
    public CompletedDialogPartBuilder? CompletedDialogPartBuilder { get; set; }
    public MessageDialogPartBuilder? MessageDialogPartBuilder { get; set; }
    public DecisionDialogPartBuilder? DecisionDialogPartBuilder { get; set; }
    public NavigationDialogPartBuilder? NavigationDialogPartBuilder { get; set; }
    public RedirectDialogPartBuilder? RedirectDialogPartBuilder { get; set; }
    public bool IsEmptyDialogPart { get; set; }

    public DialogPartBuilder()
    {
    }

    public DialogPartBuilder(IDialogPart dialogPart)
    {
        if (dialogPart is IQuestionDialogPart questionDialogPart) QuestionDialogPartBuilder = new QuestionDialogPartBuilder(questionDialogPart);
        else if (dialogPart is IAbortedDialogPart abortedDialogPart) AbortedDialogPartBuilder = new AbortedDialogPartBuilder(abortedDialogPart);
        else if (dialogPart is IErrorDialogPart errorDialogPart) ErrorDialogPartBuilder = new ErrorDialogPartBuilder(errorDialogPart);
        else if (dialogPart is ICompletedDialogPart completedDialogPart) CompletedDialogPartBuilder = new CompletedDialogPartBuilder(completedDialogPart);
        else if (dialogPart is IMessageDialogPart messageDialogPart) MessageDialogPartBuilder = new MessageDialogPartBuilder(messageDialogPart);
        else if (dialogPart is IDecisionDialogPart decisionDialogPart) DecisionDialogPartBuilder = new DecisionDialogPartBuilder(decisionDialogPart);
        else if (dialogPart is INavigationDialogPart navigationDialogPart) NavigationDialogPartBuilder = new NavigationDialogPartBuilder(navigationDialogPart);
        else if (dialogPart is IRedirectDialogPart redirectDialogPart) RedirectDialogPartBuilder = new RedirectDialogPartBuilder(redirectDialogPart);
        else if (dialogPart.GetType().Name == "EmptyDialogPart") IsEmptyDialogPart = true;
        else throw new ArgumentException($"Dialogpart type [{dialogPart.GetType().FullName}] is not supported");
    }

    public DialogPartBuilder(QuestionDialogPartBuilder questionDialogPartBuilder) => QuestionDialogPartBuilder = questionDialogPartBuilder;
    public DialogPartBuilder(AbortedDialogPartBuilder abortedDialogPartBuilder) => AbortedDialogPartBuilder = abortedDialogPartBuilder;
    public DialogPartBuilder(ErrorDialogPartBuilder errorDialogPartBuilder) => ErrorDialogPartBuilder = errorDialogPartBuilder;
    public DialogPartBuilder(CompletedDialogPartBuilder completedDialogPartBuilder) => CompletedDialogPartBuilder = completedDialogPartBuilder;
    public DialogPartBuilder(MessageDialogPartBuilder messageDialogPartBuilder) => MessageDialogPartBuilder = messageDialogPartBuilder;
    public DialogPartBuilder(DecisionDialogPartBuilder decisionDialogPartBuilder) => DecisionDialogPartBuilder = decisionDialogPartBuilder;
    public DialogPartBuilder(NavigationDialogPartBuilder navigationDialogPartBuilder) => NavigationDialogPartBuilder = navigationDialogPartBuilder;
    public DialogPartBuilder(RedirectDialogPartBuilder redirectDialogPartBuilder) => RedirectDialogPartBuilder = redirectDialogPartBuilder;

    public IDialogPart Build()
    {
        if (QuestionDialogPartBuilder != null) return QuestionDialogPartBuilder.Build();
        else if (AbortedDialogPartBuilder != null) return AbortedDialogPartBuilder.Build();
        else if (ErrorDialogPartBuilder != null) return ErrorDialogPartBuilder.Build();
        else if (CompletedDialogPartBuilder != null) return CompletedDialogPartBuilder.Build();
        else if (MessageDialogPartBuilder != null) return MessageDialogPartBuilder.Build();
        else if (DecisionDialogPartBuilder != null) return DecisionDialogPartBuilder.Build();
        else if (NavigationDialogPartBuilder != null) return NavigationDialogPartBuilder.Build();
        else if (RedirectDialogPartBuilder != null) return RedirectDialogPartBuilder.Build();
        else if (IsEmptyDialogPart) return new EmptyDialogPart();
        else throw new NotSupportedException("No valid type was found to build");
    }

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState GetState() => DialogState.Initial;
    }
}
