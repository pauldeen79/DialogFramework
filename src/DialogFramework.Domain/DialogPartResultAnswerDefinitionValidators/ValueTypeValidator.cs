namespace DialogFramework.Domain.DialogPartResultAnswerDefinitionValidators;

public class ValueTypeValidator : IDialogPartResultAnswerDefinitionValidator
{
    public Type Type { get; }
    public ValueTypeValidator(Type type) => Type = type;

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IDialogPart dialogPart,
                                                         IDialogPartResultAnswerDefinition dialogPartResultDefinition,
                                                         IEnumerable<IDialogPartResultAnswer> dialogPartResults)
    {
        if (dialogPartResults.Any(x => x.Value.Value != null && !Type.IsInstanceOfType(x.Value.Value)))
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is not of type [{Type.FullName}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id  }));
        }
    }
}
