namespace DialogFramework.Core.DomainModel;

public record ProvidedAnswer : IProvidedAnswer
{
    public ProvidedAnswer(IQuestionDialogPart question,
                          IQuestionDialogPartAnswer answer,
                          object? value)
    {
        Question = question;
        Answer = answer;
        Value = value;
    }

    public IQuestionDialogPart Question { get; }
    public IQuestionDialogPartAnswer Answer { get; }
    public object? Value { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Question.Answers.Any(a => a.Id == Answer.Id))
        {
            yield return new ValidationResult($"Unknown answer: [{Answer.Id}]");
        }
    }
}
