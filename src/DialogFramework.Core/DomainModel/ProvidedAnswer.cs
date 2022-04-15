namespace DialogFramework.Core.DomainModel;

public record ProvidedAnswer : IProvidedAnswer
{
    /// <summary>
    /// Constructs a provided answer from a non-question dialog part.
    /// </summary>
    public ProvidedAnswer(IDialogPart dialogPart)
        : this(dialogPart, new NonQuestionDialogPartAnswer(), new EmptyAnswerValue())
    {
        
    }

    /// <summary>
    /// Constructs a provided answer from a question dialog part, without a value.
    /// </summary>
    public ProvidedAnswer(IDialogPart dialogPart,
                          IQuestionDialogPartAnswer answer)
        : this(dialogPart, answer, new EmptyAnswerValue())
    {
    }

    /// <summary>
    /// Constructs a provided answer from a question dialog part, with a value.
    /// </summary>
    public ProvidedAnswer(IDialogPart dialogPart,
                          IQuestionDialogPartAnswer answer,
                          IProvidedAnswerValue answerValue)
    {
        DialogPart = dialogPart;
        Answer = answer;
        AnswerValue = answerValue;
    }

    public IDialogPart DialogPart { get; }
    public IQuestionDialogPartAnswer Answer { get; }
    public IProvidedAnswerValue AnswerValue { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var questionDialogPart = DialogPart as IQuestionDialogPart;
        if (questionDialogPart != null && !questionDialogPart.Answers.Any(a => a.Id == Answer.Id))
        {
            yield return new ValidationResult($"Unknown answer: [{Answer.Id}]");
        }
    }
}
