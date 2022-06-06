namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public IDialogPartIdentifier GetNextPartId(IDialog context) => NavigateToId;
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
}
