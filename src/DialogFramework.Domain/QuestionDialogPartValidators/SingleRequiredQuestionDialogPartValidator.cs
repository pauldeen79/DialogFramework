namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class SingleRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialogDefinition dialog,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var answerCount = dialogPartResults.Count(x => Equals(x.DialogPartId, context.CurrentPartId) && !string.IsNullOrEmpty(x.ResultId.Value));
        if (answerCount == 0)
        {
            yield return new DialogValidationResult("Answer is required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
        else if (answerCount > 1)
        {
            yield return new DialogValidationResult("Only one answer is allowed", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
