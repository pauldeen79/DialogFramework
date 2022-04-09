namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IMessageDialogPart : IGroupedDialogPart
{
    string Message { get; }
}
