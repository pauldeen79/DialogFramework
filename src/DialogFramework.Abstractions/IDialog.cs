namespace DialogFramework.Abstractions;

public interface IDialog
{
    IDialogIdentifier Id { get; }
    IDialogDefinitionIdentifier CurrentDialogId { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    string? ErrorMessage { get; }

    Result Start(IDialogDefinition definition, IConditionEvaluator evaluator);

    Result Continue(IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> answers, IConditionEvaluator evaluator);

    Result Abort(IDialogDefinition definition, IConditionEvaluator evaluator);

    Result Error(IDialogDefinition definition, IConditionEvaluator evaluator, IError? error);

    Result NavigateTo(IDialogDefinition definition, IDialogPartIdentifier navigateToPartId, IConditionEvaluator evaluator);

    Result ResetState(IDialogDefinition definition, IDialogPartIdentifier partId);

    Result<IEnumerable<IDialogPartResult>> GetDialogPartResultsByPartIdentifier(IDialogPartIdentifier dialogPartId);

    void AddProperty(IProperty property);
}
