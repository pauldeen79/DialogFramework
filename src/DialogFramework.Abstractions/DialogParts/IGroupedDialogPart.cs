namespace DialogFramework.Abstractions.DialogParts;

public interface IGroupedDialogPart : IDialogPart
{
    IDialogPartGroup Group { get; }
    string Heading { get; }
}
