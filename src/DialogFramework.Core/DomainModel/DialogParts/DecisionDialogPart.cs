namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record DecisionDialogPart : IDecisionDialogPart
{
    protected DecisionDialogPart(string id) => Id = id;

    public abstract IDialogPart GetNextPart(IDialogContext context);
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
