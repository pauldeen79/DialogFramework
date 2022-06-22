namespace DialogFramework.Abstractions;

public interface IDialogPartResultDefinitionValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition dialogDefinition,
                                                  IDialogPart dialogPart,
                                                  IDialogPartResultDefinition dialogPartResultDefinition,
                                                  IEnumerable<IDialogPartResultAnswer> dialogPartResults);
}
