namespace DialogFramework.Domain.DomainModel.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var submittedPartCount = dialogPartResults
            .Where(x => !string.IsNullOrEmpty(x.ResultId))
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (dialog.GetPartById(context.CurrentPart.Id) as IQuestionDialogPart)?.Results?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<string>());
        }
    }
}
