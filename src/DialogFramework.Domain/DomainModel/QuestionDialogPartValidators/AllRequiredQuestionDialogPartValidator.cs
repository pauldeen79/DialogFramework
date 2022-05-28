namespace DialogFramework.Domain.DomainModel.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IConditionEvaluator conditionEvaluator,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var submittedPartCount = dialogPartResults
            .Where(x => !string.IsNullOrEmpty(x.ResultId))
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (dialog.GetPartById(context, context.CurrentPart.Id, conditionEvaluator) as IQuestionDialogPart)?.Results?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<string>());
        }
    }
}
