namespace DialogFramework.Core.DomainModel;

public record Dialog : IDialog
{
    public Dialog(IDialogMetadata metadata,
                  IEnumerable<IDialogPart> parts,
                  IErrorDialogPart errorDialogPart,
                  IAbortedDialogPart abortedPart,
                  ICompletedDialogPart completedPart,
                  IEnumerable<IDialogPartGroup> partGroups)
    {
        Metadata = metadata;
        Parts = new ValueCollection<IDialogPart>(parts);
        ErrorPart = errorDialogPart;
        AbortedPart = abortedPart;
        CompletedPart = completedPart;
        PartGroups = new ValueCollection<IDialogPartGroup>(partGroups);
    }

    public IDialogMetadata Metadata { get; }
    public ValueCollection<IDialogPart> Parts { get; }
    public IErrorDialogPart ErrorPart { get; }
    public IAbortedDialogPart AbortedPart { get; }
    public ICompletedDialogPart CompletedPart { get; }
    public ValueCollection<IDialogPartGroup> PartGroups { get; }
}
