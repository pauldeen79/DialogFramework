namespace DialogFramework.Abstractions.DomainModel;

public interface IQuestionDialogPartValidator
{
    IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                  IDialog dialog,
                                                  IConditionEvaluator conditionEvaluator,
                                                  IEnumerable<IDialogPartResult> dialogPartResults);
}
