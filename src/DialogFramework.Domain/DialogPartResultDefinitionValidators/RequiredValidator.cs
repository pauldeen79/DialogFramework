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
        if (!dialogPartResults.Any()
            || dialogPartResults.Any(x => x.Value.Value == null || x.Value.Value is string s && string.IsNullOrEmpty(s)))
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is required", new ReadOnlyValueCollection<string>(new[] { dialogPartResultDefinition.Id }));
        }
        else if (_checkForSingleOccurence && dialogPartResults.Count() > 1)
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is only allowed one time", new ReadOnlyValueCollection<string>(new[] { dialogPartResultDefinition.Id }));
        }
    }
}
