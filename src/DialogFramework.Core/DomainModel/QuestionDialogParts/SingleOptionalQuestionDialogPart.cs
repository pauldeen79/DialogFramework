namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record SingleOptionalQuestionDialogPart : QuestionDialogPart
{
    public SingleOptionalQuestionDialogPart(string id,
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
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.Result.Id));
        if (answerCount > 1)
        {
            ErrorMessages.Add("Only one answer is allowed");
        }
    }
}
