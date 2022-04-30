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
        var submittedPartCount = dialogPartResults.Where(x => !string.IsNullOrEmpty(x.ResultId)).GroupBy(x => x.ResultId).Count();
        if (submittedPartCount != Results.Count)
        {
            ErrorMessages.Add($"All {Results.Count} answers are required");
        }
    }
}
