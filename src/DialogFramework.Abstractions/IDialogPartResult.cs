namespace DialogFramework.Abstractions;

public interface IDialogPartResult 
{
    IDialogDefinitionIdentifier DialogId { get; }
    IDialogPartIdentifier DialogPartId { get; }
    IDialogPartResultIdentifier ResultId { get; }
    IDialogPartResultValueAnswer Value { get; }
}
