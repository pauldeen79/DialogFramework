namespace DialogFramework.Abstractions;

public interface IDialogPart
{
    string Id { get; }
    DialogState GetState();
}
