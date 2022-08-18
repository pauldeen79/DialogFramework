namespace DialogFramework.Abstractions;

public interface IDialogPartResultAnswerDefinitionValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition dialogDefinition,
                                                  IDialogPart dialogPart,
                                                  IDialogPartResultAnswerDefinition dialogPartResultDefinition,
                                                  IEnumerable<IDialogPartResultAnswer> dialogPartResults);
}
