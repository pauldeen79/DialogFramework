namespace DialogFramework.Core.Tests.Fixtures;

public record SimpleFormFlowDialog : IDialog
{
    public IDialogMetadata Metadata => new DialogMetadata(nameof(TestFlowDialog), "Simple fom flow dialog", "1.0.0", true);

    public ValueCollection<IDialogPart> Parts { get; }
    public IErrorDialogPart ErrorPart { get; }
    public IAbortedDialogPart AbortedPart { get; }
    public ICompletedDialogPart CompletedPart { get; }
    public ValueCollection<IDialogPartGroup> PartGroups { get; }

    public SimpleFormFlowDialog()
    {
        var getInformationGroup = new DialogPartGroup("Get information", "Get information", 1);
        var completedGroup = new DialogPartGroup("Completed", "Completed", 2);
        Parts = new ValueCollection<IDialogPart>
        (
            new IDialogPart[]
            {
                new AllRequiredQuestionDialogPart
                (
                    "ContactInfo",
                    "Contact information",
                    "Please provide your e-mail address and telephone number, so we can contact you.",
                    getInformationGroup,
                    new IDialogPartResultDefinition[]
                    {
                        new DialogPartResultDefinition("EmailAddress", "E-mail address", ResultValueType.Text),
                        new DialogPartResultDefinition("TelephoneNumber", "Telephone number", ResultValueType.Text),
                        new DialogPartResultDefinition("SignUpForNewsletter", "Subscribe to newsletter (optional)", ResultValueType.YesNo)
                    }
                )
            }
        );
        ErrorPart = new ErrorDialogPart("Error", "Something went wrong. Please try again, or contact us in case the problem persists.", null);
        AbortedPart = new AbortedDialogPart("Aborted", "The dialog is aborted. You can come back any time to start the application again.");
        CompletedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for using this application. Please come back soon!", completedGroup);
        PartGroups = new ValueCollection<IDialogPartGroup>(new[] { getInformationGroup, completedGroup });
    }

    public IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                         IEnumerable<IDialogPartResult> newDialogPartResults)
    {
        var dialogPartIds = newDialogPartResults.GroupBy(x => x.DialogPartId).Select(x => x.Key).ToArray();
        return existingDialogPartResults.Where(x => !dialogPartIds.Contains(x.DialogPartId)).Concat(newDialogPartResults);
    }

    public IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults, IDialogPart currentPart)
        => existingDialogPartResults.Where(x => x.DialogPartId != currentPart.Id);

    public bool CanNavigateTo(IDialogPart currentPart, IDialogPart navigateToPart, IEnumerable<IDialogPartResult> existingDialogPartResults)
        => currentPart.Id == navigateToPart.Id || existingDialogPartResults.Any(x => x.DialogPartId == navigateToPart.Id);
}
