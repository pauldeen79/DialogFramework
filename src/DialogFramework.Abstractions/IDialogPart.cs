namespace DialogFramework.Abstractions;

public interface IDialogPart
{
    IDialogPartIdentifier Id { get; }
    bool SupportsReset { get; }
    DialogState GetState();
    IDialogPartBuilder CreateBuilder();
}
