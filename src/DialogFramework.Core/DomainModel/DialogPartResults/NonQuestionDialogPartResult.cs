namespace DialogFramework.Core.DomainModel.DialogPartResults;

public class NonQuestionDialogPartResult : IQuestionDialogPartResult
{
    public string Id => string.Empty;
    public string Title => string.Empty;
    public AnswerValueType ValueType => AnswerValueType.None;
}
