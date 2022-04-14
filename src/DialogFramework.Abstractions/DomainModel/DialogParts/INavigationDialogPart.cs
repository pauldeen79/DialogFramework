namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface INavigationDialogPart : IDialogPart
{
    IDialogPart GetNextPart(IDialogContext context);
}
