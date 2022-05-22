namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record DecisionDialogPart : IDecisionDialogPart
{
    protected DecisionDialogPart(string id) => Id = id;

    public abstract string GetNextPartId(IDialogContext context, IDialog dialog);
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
