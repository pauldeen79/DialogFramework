namespace DialogFramework.Abstractions.DomainModel;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialog dialog,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
