namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IGroupedDialogPart : IDialogPart
{
    IDialogPartGroup Group { get; }
}
