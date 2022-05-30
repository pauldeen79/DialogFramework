namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class SingleRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.ResultId.Value)); //TODO: Find a way to check for empty result id
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
