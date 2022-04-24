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

    public virtual bool CanNavigateTo(IDialogPart currentPart,
                                      IDialogPart navigateToPart,
                                      IEnumerable<IDialogPartResult> existingDialogPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        return currentPart.Id == navigateToPart.Id || existingDialogPartResults.Any(x => x.DialogPart.Id == navigateToPart.Id);
    }

    public virtual IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                                 IEnumerable<IDialogPartResult> newDialogPartResults)
    {
        // Decision: By default, only the results from the requested part are replaced.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        var dialogPartIds = newDialogPartResults.GroupBy(x => x.DialogPart.Id).Select(x => x.Key).ToArray();
        return existingDialogPartResults.Where(x => !dialogPartIds.Contains(x.DialogPart.Id)).Concat(newDialogPartResults);
    }

    public virtual IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                                              IDialogPart currentPart)
    {
        // Decision: By default, only remove the results from the requested part.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return existingDialogPartResults.Where(x => x.DialogPart.Id != currentPart.Id);
    }
}
