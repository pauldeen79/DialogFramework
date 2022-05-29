namespace DialogFramework.Domain.TestData;

public static class QuestionDialogPartFixture
{
    public static QuestionDialogPartBuilder CreateBuilder()
        => new QuestionDialogPartBuilder()
            .WithId("Test")
            .WithTitle("Give me an answer!")
            .WithHeading("Title")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .AddResults
            (
                new DialogPartResultDefinitionBuilder()
                    .WithId("A")
                    .WithTitle("First")
                    .WithValueType(ResultValueType.YesNo),
                new DialogPartResultDefinitionBuilder()
                    .WithId("B")
                    .WithTitle("Second")
                    .WithValueType(ResultValueType.YesNo)
            );

    public static IQuestionDialogPart Validate(IDialogPart instance,
                                               IDialogContext context,
                                               IDialog dialog,
                                               IEnumerable<IDialogPartResult> providedAnswers)
        => instance.Validate(context, dialog, providedAnswers) as IQuestionDialogPart
            ?? new QuestionDialogPartBuilder().WithGroup(new DialogPartGroupBuilder()).Build();
}
