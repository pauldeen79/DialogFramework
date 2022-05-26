namespace DialogFramework.Domain.DomainModel.QuestionDialogPartValidators;

public class SingleRequiredQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.ResultId));
        if (answerCount == 0)
        {
            yield return new DialogValidationResult("Answer is required", new ValueCollection<string>());
        }
        else if (answerCount > 1)
        {
            yield return new DialogValidationResult("Only one answer is allowed", new ValueCollection<string>());
        }
    }
}
