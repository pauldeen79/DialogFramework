namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultDefinition
{
    string Id { get; }
    string Title { get; }
    ResultValueType ValueType { get; }
    IReadOnlyCollection<IDialogPartResultDefinitionValidator> Validators { get; }
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialog dialog,
                                                  IDialogPart dialogPart,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
