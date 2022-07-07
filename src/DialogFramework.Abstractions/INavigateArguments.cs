namespace DialogFramework.Abstractions;

public interface INavigateArguments
{
    IDialogIdentifier CurrentDialogId { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    string? ErrorMessage { get;  }

    DialogAction Action { get; }
    IDialogDefinition DialogDefinition { get; }
    IConditionEvaluator ConditionEvaluator { get; }

    void AddProperty(IProperty property);
}
