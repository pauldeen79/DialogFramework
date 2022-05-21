namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface INavigationDialogPart : IDialogPart
{
    string GetNextPartId(IDialogContext context);
}
