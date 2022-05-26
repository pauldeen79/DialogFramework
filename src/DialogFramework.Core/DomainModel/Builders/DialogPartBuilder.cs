namespace DialogFramework.Core.DomainModel.Builders;

public class DialogPartBuilder
{
    private readonly QuestionDialogPartBuilder? _questionDialogPartBuilder;
    private readonly AbortedDialogPartBuilder? _abortedDialogPartBuilder;
    private readonly ErrorDialogPartBuilder? _errorDialogPartBuilder;
    private readonly CompletedDialogPartBuilder? _completedDialogPartBuilder;
    private readonly MessageDialogPartBuilder? _messageDialogPartBuilder;
    private readonly DecisionDialogPartBuilder? _decisionDialogPartBuilder;
    private readonly NavigationDialogPartBuilder? _navigationDialogPartBuilder;
    private readonly RedirectDialogPartBuilder? _redirectDialogPartBuilder;

    public DialogPartBuilder(IDialogPart dialogPart)
    {
        if (dialogPart is IQuestionDialogPart questionDialogPart)
        {
            _questionDialogPartBuilder = new QuestionDialogPartBuilder(questionDialogPart);
        }
        else if (dialogPart is IAbortedDialogPart abortedDialogPart)
        {
            _abortedDialogPartBuilder = new AbortedDialogPartBuilder(abortedDialogPart);
        }
        else if (dialogPart is IErrorDialogPart errorDialogPart)
        {
            _errorDialogPartBuilder = new ErrorDialogPartBuilder(errorDialogPart);
        }
        else if (dialogPart is ICompletedDialogPart completedDialogPart)
        {
            _completedDialogPartBuilder = new CompletedDialogPartBuilder(completedDialogPart);
        }
        else if (dialogPart is IMessageDialogPart messageDialogPart)
        {
            _messageDialogPartBuilder = new MessageDialogPartBuilder(messageDialogPart);
        }
        else if (dialogPart is IDecisionDialogPart decisionDialogPart)
        {
            _decisionDialogPartBuilder = new DecisionDialogPartBuilder(decisionDialogPart);
        }
        else if (dialogPart is INavigationDialogPart navigationDialogPart)
        {
            _navigationDialogPartBuilder = new NavigationDialogPartBuilder(navigationDialogPart);
        }
        else if (dialogPart is IRedirectDialogPart redirectDialogPart)
        {
            _redirectDialogPartBuilder = new RedirectDialogPartBuilder(redirectDialogPart);
        }
        else
        {
            throw new ArgumentException($"Dialogpart type [{dialogPart.GetType().FullName}] is not supported");
        }
    }

    public DialogPartBuilder(QuestionDialogPartBuilder questionDialogPartBuilder) => _questionDialogPartBuilder = questionDialogPartBuilder;
    public DialogPartBuilder(AbortedDialogPartBuilder abortedDialogPartBuilder) => _abortedDialogPartBuilder = abortedDialogPartBuilder;
    public DialogPartBuilder(ErrorDialogPartBuilder errorDialogPartBuilder) => _errorDialogPartBuilder = errorDialogPartBuilder;
    public DialogPartBuilder(CompletedDialogPartBuilder completedDialogPartBuilder) => _completedDialogPartBuilder = completedDialogPartBuilder;
    public DialogPartBuilder(MessageDialogPartBuilder messageDialogPartBuilder) => _messageDialogPartBuilder = messageDialogPartBuilder;
    public DialogPartBuilder(DecisionDialogPartBuilder decisionDialogPartBuilder) => _decisionDialogPartBuilder = decisionDialogPartBuilder;
    public DialogPartBuilder(NavigationDialogPartBuilder navigationDialogPartBuilder) => _navigationDialogPartBuilder = navigationDialogPartBuilder;
    public DialogPartBuilder(RedirectDialogPartBuilder redirectDialogPartBuilder) => _redirectDialogPartBuilder = redirectDialogPartBuilder;

    public IDialogPart Build()
    {
        if (_questionDialogPartBuilder != null) return _questionDialogPartBuilder.Build();
        else if (_abortedDialogPartBuilder != null) return _abortedDialogPartBuilder.Build();
        else if (_errorDialogPartBuilder != null) return _errorDialogPartBuilder.Build();
        else if (_completedDialogPartBuilder != null) return _completedDialogPartBuilder.Build();
        else if (_messageDialogPartBuilder != null) return _messageDialogPartBuilder.Build();
        else if (_decisionDialogPartBuilder != null) return _decisionDialogPartBuilder.Build();
        else if (_navigationDialogPartBuilder != null) return _navigationDialogPartBuilder.Build();
        else if (_redirectDialogPartBuilder != null) return _redirectDialogPartBuilder.Build();
        else throw new NotSupportedException("No valid type was found to build");
    }
}
