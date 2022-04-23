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

    public virtual IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                                 IEnumerable<IDialogPartResult> newDialogPartResults)
    {
        var dialogPartIds = newDialogPartResults.GroupBy(x => x.DialogPart.Id).Select(x => x.Key).ToArray();
        return existingDialogPartResults.Where(x => !dialogPartIds.Contains(x.DialogPart.Id)).Concat(newDialogPartResults);
    }
}
