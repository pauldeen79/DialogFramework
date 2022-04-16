namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record SingleRequiredQuestionDialogPart : QuestionDialogPart
{
    public SingleRequiredQuestionDialogPart(string id,
                                            string heading,
                                            string message,
                                            IDialogPartGroup group,
                                            IEnumerable<IDialogPartResultDefinition> results)
        : base(id, heading, message, group, results)
    {
    }

    protected override void HandleValidate(IEnumerable<IDialogPartResult> dialogPartResults)
    {
        base.HandleValidate(dialogPartResults);
        var answerCount = dialogPartResults.Count();
        if (answerCount == 0)
        {
            ErrorMessages.Add("Answer is required");
        }
        else if (answerCount > 1)
        {
            ErrorMessages.Add("Only one answer is allowed");
        }
    }
}
