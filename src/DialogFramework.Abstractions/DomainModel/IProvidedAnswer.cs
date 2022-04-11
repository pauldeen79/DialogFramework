namespace DialogFramework.Abstractions.DomainModel;

public interface IProvidedAnswer : IValidatableObject
{
    IQuestionDialogPart Question { get; }
    IQuestionDialogPartAnswer Answer { get; }
    object? Value { get; }
}
