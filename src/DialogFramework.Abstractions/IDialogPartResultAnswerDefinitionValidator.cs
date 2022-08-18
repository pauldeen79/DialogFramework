namespace DialogFramework.Abstractions;

public interface IDialogPartResultAnswerDefinitionValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition definition,
                                                  IDialogPart part,
                                                  IDialogPartResultAnswerDefinition answerDefinition,
                                                  IEnumerable<IDialogPartResultAnswer> answers);
}
