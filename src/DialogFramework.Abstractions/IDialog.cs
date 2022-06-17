namespace DialogFramework.Abstractions;

public interface IDialog
{
    IDialogIdentifier Id { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }

    Result Start(IDialogDefinition definition, IConditionEvaluator evaluator);
    Result Continue(IDialogDefinition definition, IEnumerable<IDialogPartResult> results, IConditionEvaluator evaluator);
    Result Abort(IDialogDefinition definition);
    Result Error(IDialogDefinition definition);
    Result NavigateTo(IDialogDefinition definition, IDialogPartIdentifier navigateToPartId);
    Result ResetCurrentState(IDialogDefinition definition);
}
