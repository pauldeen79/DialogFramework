namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class AllRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
    {
        var submittedPartCount = answers
            .Where(x => !string.IsNullOrEmpty(x.ResultId.Value))
            .GroupBy(x => x.ResultId)
            .Count();
        var definedResultCount = (definition.GetPartById(dialog.CurrentPartId).Value as IQuestionDialogPart)?.Answers?.Count ?? 0;
        if (submittedPartCount != definedResultCount)
        {
            yield return new DialogValidationResult($"All {definedResultCount} answers are required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
