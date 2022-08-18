﻿namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class QuestionDialogPartFixture
{
    public static QuestionDialogPartBuilder CreateBuilder()
        => new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithTitle("Give me an answer!")
            .WithHeading("Title")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .AddAnswers
            (
                new DialogPartResultAnswerDefinitionBuilder()
                    .WithId(new DialogPartResultIdentifierBuilder().WithValue("A"))
                    .WithTitle("First")
                    .WithValueType(ResultValueType.YesNo),
                new DialogPartResultAnswerDefinitionBuilder()
                    .WithId(new DialogPartResultIdentifierBuilder().WithValue("B"))
                    .WithTitle("Second")
                    .WithValueType(ResultValueType.YesNo)
            );
}
