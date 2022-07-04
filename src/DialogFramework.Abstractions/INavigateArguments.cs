namespace DialogFramework.Abstractions;

public interface INavigateArguments
{
    IDialogIdentifier CurrentDialogId { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    string? ErrorMessage { get; }
    DialogAction Action { get; }
    IConditionEvaluator ConditionEvaluator { get; }

    Result? Result { get; set; }

    void AddProperty(IProperty property);
}
