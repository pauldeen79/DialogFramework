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
                                                         IDialogDefinition dialogDefinition,
                                                         IDialogPart dialogPart,
                                                         IDialogPartResultAnswerDefinition dialogPartResultDefinition,
                                                         IEnumerable<IDialogPartResultAnswer> dialogPartResults)
    {
        var dialogPartResultsArray = dialogPartResults.ToArray();
        if (!dialogPartResultsArray.Any()
            || dialogPartResultsArray.Any(x => x.Value.Value == null || x.Value.Value is string s && string.IsNullOrEmpty(s)))
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id }));
        }
        else if (_checkForSingleOccurence && dialogPartResultsArray.Count() > 1)
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is only allowed one time", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id }));
        }
    }
}
