namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultDefinition
{
    string Id { get; }
    string Title { get; }
    ResultValueType ValueType { get; }
    ValueCollection<IDialogPartResultDefinitionValidator> Validators { get; }
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialogPart dialogPart,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
