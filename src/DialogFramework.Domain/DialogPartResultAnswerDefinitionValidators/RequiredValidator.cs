namespace DialogFramework.Domain.DialogPartResultAnswerDefinitionValidators;

public class RequiredValidator : IDialogPartResultAnswerDefinitionValidator
{
    private readonly bool _checkForSingleOccurence;

    public RequiredValidator() : this(false)
    {
    }

    public RequiredValidator(bool checkForSingleOccurence)
        => _checkForSingleOccurence = checkForSingleOccurence;

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IDialogPart part,
                                                         IDialogPartResultAnswerDefinition answerDefinition,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
    {
        var answersArray = answers.ToArray();
        if (!answersArray.Any()
            || answersArray.Any(x => x.Value.Value == null || x.Value.Value is string s && string.IsNullOrEmpty(s)))
        {
            yield return new DialogValidationResult($"Answer value of [{part.Id}.{answerDefinition.Id}] is required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { answerDefinition.Id }));
        }
        else if (_checkForSingleOccurence && answersArray.Count() > 1)
        {
            yield return new DialogValidationResult($"Answer value of [{part.Id}.{answerDefinition.Id}] is only allowed one time", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { answerDefinition.Id }));
        }
    }
}
