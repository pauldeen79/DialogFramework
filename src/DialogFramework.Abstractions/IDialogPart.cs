namespace DialogFramework.Abstractions;

public interface IDialogPart
{
    IDialogPartIdentifier Id { get; }
    DialogState GetState();
}
