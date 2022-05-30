namespace DialogFramework.Abstractions.DialogParts;

public interface ICompletedDialogPart : IGroupedDialogPart
{
    string Message { get; }
}
