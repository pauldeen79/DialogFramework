namespace DialogFramework.Abstractions.DomainModel;

public interface IProvidedAnswer : IValidatableObject
{
    IDialogPart DialogPart { get; }
    IQuestionDialogPartAnswer Answer { get; }
    IProvidedAnswerValue AnswerValue { get; }
}
