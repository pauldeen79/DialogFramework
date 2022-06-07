namespace DialogFramework.Abstractions;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition dialogDefinition,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
