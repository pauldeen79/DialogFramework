namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public IDialogPartIdentifier GetNextPartId(IDialogContext context) => NavigateToId;
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
}
