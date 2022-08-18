namespace DialogFramework.Abstractions;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition definition,
                                                  IEnumerable<IDialogPartResultAnswer> answers);
}
