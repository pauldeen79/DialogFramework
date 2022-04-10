namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPart
{
    string Id { get; }
    DialogState State { get; }
}
