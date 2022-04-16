namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions;

public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
{
    public string Id => string.Empty;
    public string Title => string.Empty;
    public AnswerValueType ValueType => AnswerValueType.None;
}
