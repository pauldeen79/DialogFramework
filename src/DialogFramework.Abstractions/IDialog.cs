namespace DialogFramework.Abstractions;

public interface IDialog
{
    IDialogIdentifier Id { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
    IReadOnlyCollection<IError> Errors { get; }

    bool CanStart(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator);
    void Start(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator);

    bool CanContinue(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> partResults);
    void Continue(IDialogDefinition dialogDefinition,
                  IEnumerable<IDialogPartResult> partResults,
                  IConditionEvaluator conditionEvaluator);

    bool CanAbort(IDialogDefinition dialogDefinition);
    void Abort(IDialogDefinition dialogDefinition);

    void Error(IDialogDefinition dialogDefinition, IEnumerable<IError> errors);

    bool CanNavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId);
    void NavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId);

    bool CanResetCurrentState(IDialogDefinition dialogDefinition);
    void ResetCurrentState(IDialogDefinition dialogDefinition);
}
