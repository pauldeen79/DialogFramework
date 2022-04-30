namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record SingleOptionalQuestionDialogPart : QuestionDialogPart
{
    public SingleOptionalQuestionDialogPart(string id,
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
        var answerCount = dialogPartResults.Count(x => !string.IsNullOrEmpty(x.ResultId));
        if (answerCount > 1)
        {
            ErrorMessages.Add("Only one answer is allowed");
        }
    }
}
