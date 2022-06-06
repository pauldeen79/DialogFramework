namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog context,
                                                         IDialogDefinition dialog,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var submittedPartCount = dialogPartResults
            .Where(x => Equals(x.DialogPartId, context.CurrentPartId) && !string.IsNullOrEmpty(x.ResultId.Value))
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (dialog.GetPartById(context.CurrentPartId) as IQuestionDialogPart)?.Results?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
