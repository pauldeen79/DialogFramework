namespace DialogFramework.Abstractions;

public interface IDialogPartResultDefinitionValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialogDefinition dialog,
                                                  IDialogPart dialogPart,
                                                  IDialogPartResultDefinition dialogPartResultDefinition,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
