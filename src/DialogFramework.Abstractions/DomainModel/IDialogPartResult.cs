namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResult : IValidatableObject
{
    IDialogPart DialogPart { get; }
    IQuestionDialogPartResult Result { get; }
    IDialogPartResultValue AnswerValue { get; }
}
