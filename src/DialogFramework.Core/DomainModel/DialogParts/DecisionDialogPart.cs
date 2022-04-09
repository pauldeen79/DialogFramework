namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record DecisionDialogPart : IDecisionDialogPart
{
    protected DecisionDialogPart(string id)
    {
        Id = id;
    }
                              
    public abstract string this[string columnName] { get; }
    public abstract IDialogPart? NextPart { get; }
    public string Id { get; }
    public abstract string Error { get; }
}
