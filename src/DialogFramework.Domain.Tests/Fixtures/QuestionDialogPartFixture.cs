namespace DialogFramework.Domain.Tests.Fixtures;

internal static class QuestionDialogPartFixture
{
    internal static QuestionDialogPartBuilder CreateBuilder()
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
}
