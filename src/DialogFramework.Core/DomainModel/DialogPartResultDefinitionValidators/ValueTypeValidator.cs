namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitionValidators;

public class ValueTypeValidator : IDialogPartResultDefinitionValidator
{
    public Type Type { get; }
    public ValueTypeValidator(Type type) => Type = type;

    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IDialogPart dialogPart,
                                                         IDialogPartResultDefinition dialogPartResultDefinition,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        if (dialogPartResults.Any(x => x.Value.Value != null && !Type.IsInstanceOfType(x.Value.Value)))
        {
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] is not of type [{Type.FullName}]", new ValueCollection<string>(new[] { dialogPartResultDefinition.Id  }));
        }
    }
}
