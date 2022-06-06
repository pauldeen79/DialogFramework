namespace DialogFramework.Abstractions;

public interface IDialogPartResultDefinition
{
    IDialogPartResultIdentifier Id { get; }
    string Title { get; }
    ResultValueType ValueType { get; }
    IReadOnlyCollection<IDialogPartResultDefinitionValidator> Validators { get; }
    IEnumerable<IDialogValidationResult> Validate(IDialog context,
                                                  IDialogDefinition dialog,
                                                  IDialogPart dialogPart,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
