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

    protected override void HandleValidate(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        base.HandleValidate(context, dialogPartResults);
        var submittedPartCount = dialogPartResults.Where(x => !string.IsNullOrEmpty(x.ResultId)).GroupBy(x => x.ResultId).Count();
        if (submittedPartCount != Results.Count)
        {
            ValidationErrors.Add(new DialogValidationResult($"All {Results.Count} answers are required"));
        }
    }
}
