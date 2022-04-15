namespace DialogFramework.Abstractions.DomainModel.DialogPartAnswers;

public class NonQuestionDialogPartAnswer : IQuestionDialogPartAnswer
{
    public string Id => string.Empty;
    public string Title => string.Empty;
    public AnswerValueType ValueType => AnswerValueType.None;
}
