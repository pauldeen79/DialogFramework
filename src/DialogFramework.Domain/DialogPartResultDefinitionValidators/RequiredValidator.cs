namespace DialogFramework.Domain.DialogPartResultDefinitionValidators;

public class RequiredValidator : IDialogPartResultDefinitionValidator
{
    private readonly bool _checkForSingleOccurence;

    public RequiredValidator() : this(false)
    {
    }

    public RequiredValidator(bool checkForSingleOccurence)
        => _checkForSingleOccurence = checkForSingleOccurence;

    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IDialogPart dialogPart,
                                                         IDialogPartResultDefinition dialogPartResultDefinition,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var filteredDialogPartResults = dialogPartResults.Where(x => Equals(x.DialogPartId, dialogPart.Id)).ToArray();
        if (!filteredDialogPartResults.Any()
            || filteredDialogPartResults.Any(x => x.Value.Value == null || x.Value.Value is string s && string.IsNullOrEmpty(s)))
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is required", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id }));
        }
        else if (_checkForSingleOccurence && filteredDialogPartResults.Count() > 1)
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is only allowed one time", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id }));
        }
    }
}
