namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class SingleOptionalQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
    {
        var answerCount = answers.Count(x => !string.IsNullOrEmpty(x.ResultId.Value));
        if (answerCount > 1)
        {
            yield return new DialogValidationResult("Only one answer is allowed", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
