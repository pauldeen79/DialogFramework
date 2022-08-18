namespace DialogFramework.Abstractions;

public interface INavigateArguments
{
    IDialogIdentifier CurrentDialogId { get; }
    IDialogDefinitionIdentifier DefinitionId { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    string? ErrorMessage { get;  }

    DialogAction Action { get; }
    IDialogDefinition Definition { get; }
    IConditionEvaluator Evaluator { get; }

    void AddProperty(IProperty property);
}
