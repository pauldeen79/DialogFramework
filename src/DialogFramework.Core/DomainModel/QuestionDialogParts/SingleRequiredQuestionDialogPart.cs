namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record SingleRequiredQuestionDialogPart : QuestionDialogPart
{
    public SingleRequiredQuestionDialogPart(string id,
                                            string heading,
                                            string title,
                                            IDialogPartGroup group,
                                            IEnumerable<IDialogPartResultDefinition> results)
        : base(id, heading, title, group, results)
    {
    }

    protected override void HandleValidate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        base.HandleValidate(context, dialog, dialogPartResults);
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.ResultId));
        if (answerCount == 0)
        {
            ValidationErrors.Add(new DialogValidationResult("Answer is required"));
        }
        else if (answerCount > 1)
        {
            ValidationErrors.Add(new DialogValidationResult("Only one answer is allowed"));
        }
    }
}
