namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialogContextIdentifier Id { get; }
    IDialogIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
    IReadOnlyCollection<IError> Errors { get; }

    bool CanStart(IDialogDefinition dialog, IConditionEvaluator conditionEvaluator);
    void Start(IDialogDefinition dialog, IConditionEvaluator conditionEvaluator);

    bool CanContinue(IDialogDefinition dialog, IEnumerable<IDialogPartResult> partResults);
    void Continue(IDialogDefinition dialog,
                  IEnumerable<IDialogPartResult> partResults,
                  IConditionEvaluator conditionEvaluator);

    bool CanAbort(IDialogDefinition dialog);
    void Abort(IDialogDefinition dialog);

    void Error(IDialogDefinition dialog, IEnumerable<IError> errors);

    bool CanNavigateTo(IDialogDefinition dialog, IDialogPartIdentifier navigateToPartId);
    void NavigateTo(IDialogDefinition dialog, IDialogPartIdentifier navigateToPartId);

    bool CanResetCurrentState(IDialogDefinition dialog);
    void ResetCurrentState(IDialogDefinition dialog);
}
