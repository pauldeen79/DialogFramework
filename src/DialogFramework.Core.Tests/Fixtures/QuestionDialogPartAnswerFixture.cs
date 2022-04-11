namespace DialogFramework.Core.Tests.Fixtures;

internal record QuestionDialogPartAnswerFixture : QuestionDialogPartAnswer
{
    public QuestionDialogPartAnswerFixture(string id,
                                           string title,
                                           AnswerValueType valueType)
        : base(id, title, valueType)
    {
    }
}
