namespace DialogFramework.Abstractions;

public interface IDialogPartResultAnswer
{
    IDialogPartResultIdentifier ResultId { get; }
    IDialogPartResultValueAnswer Value { get; }
}
