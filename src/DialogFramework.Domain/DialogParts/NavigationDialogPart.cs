namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public IDialogPartIdentifier GetNextPartId(IDialog dialog) => NavigateToId;
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
    public bool SupportsReset => false;
}
