namespace DialogFramework.Abstractions;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialogDefinition dialog,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
