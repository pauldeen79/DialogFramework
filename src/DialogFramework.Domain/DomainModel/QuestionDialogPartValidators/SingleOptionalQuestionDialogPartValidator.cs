namespace DialogFramework.Domain.DomainModel.QuestionDialogPartValidators;

public class SingleOptionalQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IConditionEvaluator conditionEvaluator,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.ResultId));
        if (answerCount > 1)
        {
            yield return new DialogValidationResult("Only one answer is allowed", new ReadOnlyValueCollection<string>());
        }
    }
}
