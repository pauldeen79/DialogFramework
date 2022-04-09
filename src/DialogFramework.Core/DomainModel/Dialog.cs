namespace DialogFramework.Core.DomainModel;

public class Dialog : IDialog
{
    public Dialog(string id,
                  string version,
                  IEnumerable<IDialogPart> parts,
                  IErrorDialogPart errorDialogPart,
                  IAbortedDialogPart abortedPart,
                  ICompletedDialogPart completedPart,
                  IEnumerable<IDialogPartGroup> partGroups)
    {
        Id = id;
        Version = version;
        Parts = new ValueCollection<IDialogPart>(parts);
        ErrorPart = errorDialogPart;
        AbortedPart = abortedPart;
        CompletedPart = completedPart;
        PartGroups = new ValueCollection<IDialogPartGroup>(partGroups);
    }

    public string Id { get; }
    public string Version { get; }
    public ValueCollection<IDialogPart> Parts { get; }
    public IErrorDialogPart ErrorPart { get; }
    public IAbortedDialogPart AbortedPart { get; }
    public ICompletedDialogPart CompletedPart { get; }
    public ValueCollection<IDialogPartGroup> PartGroups { get; }
}
