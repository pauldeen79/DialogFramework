namespace DialogFramework.Abstractions;

public interface INavigateArguments
{
    IDialogIdentifier CurrentDialogId { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; set; }
    IDialogPartIdentifier CurrentPartId { get; set; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; set; }
    DialogState CurrentState { get; set; }
    string? ErrorMessage { get; set; }
    Result? Result { get; set; }

    DialogAction Action { get; }
    IConditionEvaluator ConditionEvaluator { get; }

    void AddProperty(IProperty property);
}
