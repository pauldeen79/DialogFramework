namespace DialogFramework.Domain.QuestionDialogPartValidators;

public class SingleOptionalQuestionDialogPartValidator : IQuestionDialogPartValidator
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var answerCount = dialogPartResults.Count(x => Equals(x.DialogPartId, dialog.CurrentPartId) && !string.IsNullOrEmpty(x.ResultId.Value));
        if (answerCount > 1)
        {
            yield return new DialogValidationResult("Only one answer is allowed", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }
    }
}
