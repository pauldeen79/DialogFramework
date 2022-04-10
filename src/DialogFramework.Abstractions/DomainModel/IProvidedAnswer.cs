namespace DialogFramework.Abstractions.DomainModel;

public interface IProvidedAnswer
{
    IQuestionDialogPart Question { get; }
    IQuestionDialogPartAnswer Answer { get; }
    object? Value { get; }
}
