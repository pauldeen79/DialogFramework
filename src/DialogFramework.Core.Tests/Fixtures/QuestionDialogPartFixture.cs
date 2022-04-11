namespace DialogFramework.Core.Tests.Fixtures;

internal record QuestionDialogPartFixture : QuestionDialogPart
{
    public QuestionDialogPartFixture(string id,
                                     string title,
                                     IDialogPartGroup group,
                                     IEnumerable<IQuestionDialogPartAnswer> answers)
        : base(id, title, group, answers)
    {
    }
}
