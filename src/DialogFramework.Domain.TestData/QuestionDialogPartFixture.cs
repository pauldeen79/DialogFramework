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
            .AddResults
            (
                new DialogPartResultDefinitionBuilder()
                    .WithId(new DialogPartResultIdentifierBuilder().WithValue("A"))
                    .WithTitle("First")
                    .WithValueType(ResultValueType.YesNo),
                new DialogPartResultDefinitionBuilder()
                    .WithId(new DialogPartResultIdentifierBuilder().WithValue("B"))
                    .WithTitle("Second")
                    .WithValueType(ResultValueType.YesNo)
            );

    public static IQuestionDialogPart Validate(IDialogPart instance,
                                               IDialog dialog,
                                               IDialogDefinition dialogDefinition,
                                               IEnumerable<IDialogPartResult> providedAnswers)
        => instance.Validate(dialog, dialogDefinition, providedAnswers) as IQuestionDialogPart
            ?? new QuestionDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder())
                .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
                .Build();
}
