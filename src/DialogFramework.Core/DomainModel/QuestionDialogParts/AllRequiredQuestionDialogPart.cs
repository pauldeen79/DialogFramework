namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record AllRequiredQuestionDialogPart : QuestionDialogPart
{
    public AllRequiredQuestionDialogPart(string id,
                                         string heading,
                                         string title,
                                         IDialogPartGroup group,
                                         IEnumerable<IDialogPartResultDefinition> results)
        : base(id, heading, title, group, results)
    {
    }

    protected override void HandleValidate(IEnumerable<IDialogPartResult> dialogPartResults)
    {
        base.HandleValidate(dialogPartResults);
        var partCount = dialogPartResults.Where(x => !string.IsNullOrEmpty(x.Result.Id)).GroupBy(x => x.DialogPart.Id).Count();
        if (partCount != Results.Count)
        {
            ErrorMessages.Add($"All {Results.Count} answers are required");
        }
    }
}
