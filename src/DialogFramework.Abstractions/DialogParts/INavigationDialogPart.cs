namespace DialogFramework.Abstractions.DialogParts;

public interface INavigationDialogPart : IDialogPart
{
    string GetNextPartId(IDialogContext context);
}
