namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface ICompletedDialogPart : IGroupedDialogPart
{
    string Message { get; }
}
