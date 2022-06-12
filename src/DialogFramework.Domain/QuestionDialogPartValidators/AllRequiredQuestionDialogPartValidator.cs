namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var submittedPartCount = dialogPartResults
            .Where(x => Equals(x.DialogPartId, dialog.CurrentPartId) && !string.IsNullOrEmpty(x.ResultId.Value))
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (dialogDefinition.GetPartById(dialog.CurrentPartId).Value as IQuestionDialogPart)?.Results?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
