namespace DialogFramework.SpecFlow.Tests.Support;

public class TableDialogPartResultAnswer : IDialogPartResultAnswer
{
    [TableAliases("Result")]
    public IDialogPartResultIdentifier ResultId { get; set; } = new DialogPartResultIdentifierBuilder().WithValue(string.Empty).Build();
    public IDialogPartResultValueAnswer Value { get; set; } = new DialogPartResultValueAnswerBuilder().Build();
}
