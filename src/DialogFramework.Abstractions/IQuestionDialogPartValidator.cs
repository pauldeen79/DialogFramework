namespace DialogFramework.Abstractions;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog context,
                                                  IDialogDefinition dialog,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
