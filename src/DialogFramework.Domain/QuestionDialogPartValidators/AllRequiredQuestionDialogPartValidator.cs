namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var submittedPartCount = dialogPartResults
            .Where(x => !string.IsNullOrEmpty(x.ResultId.Value)) // TODO: Find a way to check for empty result id
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (dialog.GetPartById(context.CurrentPartId) as IQuestionDialogPart)?.Results?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
