namespace DialogFramework.Abstractions.DialogParts;

public interface INavigationDialogPart : IDialogPart
{
    IDialogPartIdentifier GetNextPartId(IDialogContext context);
}
