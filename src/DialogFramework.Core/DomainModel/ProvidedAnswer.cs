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
}
