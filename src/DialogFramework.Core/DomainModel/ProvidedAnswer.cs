namespace DialogFramework.Core.DomainModel;

public record ProvidedAnswer : IProvidedAnswer
{
    public ProvidedAnswer(IQuestionDialogPart question,
                          IQuestionDialogPartAnswer answer)
        : this(question, answer, new EmptyAnswer())
    {
    }

    public ProvidedAnswer(IQuestionDialogPart question,
                          IQuestionDialogPartAnswer answer,
                          IProvidedAnswerValue answerValue)
    {
        Question = question;
        Answer = answer;
        AnswerValue = answerValue;
    }

    public IQuestionDialogPart Question { get; }
    public IQuestionDialogPartAnswer Answer { get; }
    public IProvidedAnswerValue AnswerValue { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Question.Answers.Any(a => a.Id == Answer.Id))
        {
            yield return new ValidationResult($"Unknown answer: [{Answer.Id}]");
        }
    }
}
