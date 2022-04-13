namespace DialogFramework.Core.Tests.Fixtures;

internal record QuestionDialogPartFixture : QuestionDialogPart
{
    public QuestionDialogPartFixture(string id,
                                     string heading,
                                     string message,
                                     IDialogPartGroup group,
                                     IEnumerable<IQuestionDialogPartAnswer> answers)
        : base(id, heading, message, group, answers)
    {
    }
}
