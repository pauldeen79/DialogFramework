﻿namespace DialogFramework.Domain.TestData;

public static class DialogFixture
{
    public static DialogBuilder CreateBuilder()
        => new DialogBuilder()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("DialogFixture"))
            .AddParts
            (
                new DialogPartBuilder(QuestionDialogPartFixture.CreateBuilder()),
                new DialogPartBuilder(new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder()
                    .WithValue("Message"))
                    .WithHeading("Message")
                    .WithMessage("This is a message")
                    .WithGroup(DialogPartGroupFixture.CreateBuilder()))
            )
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .WithAbortedPart(new AbortedDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Abort")).WithMessage("Aborted"))
            .WithCompletedPart(new CompletedDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Completed")).WithMessage("Thank you").WithGroup(DialogPartGroupFixture.CreateBuilder()))
            .WithErrorPart(new ErrorDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Error")).WithErrorMessage("Something went wrong"));

    public static DialogBuilder CreateHowDoYouFeelBuilder(bool addParts = true)
    {
        var group1 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part1")).WithTitle("Give information").WithNumber(1);
        var group2 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part2")).WithTitle("Completed").WithNumber(2);
        var welcomePart = new MessageDialogPartBuilder().WithHeading("Welcome").WithId(new DialogPartIdentifierBuilder().WithValue("Welcome")).WithMessage("Welcome! I would like to answer a question").WithGroup(group1);
        var errorDialogPart = new ErrorDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Error")).WithErrorMessage("Something went horribly wrong!");
        var abortedPart = new AbortedDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Abort")).WithMessage("Dialog has been aborted");
        var answerGreat = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Great")).WithTitle("I feel great, thank you!");
        var answerOkay = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Okay")).WithTitle("I feel kind of okay");
        var answerTerrible = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Terrible")).WithTitle("I feel terrible, don't want to talk about it");
        var questionPart = new QuestionDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Question1")).WithHeading("How do you feel").WithTitle("Please tell us how you feel").WithGroup(group1).AddResults(answerGreat, answerOkay, answerTerrible);
        var messagePart = new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Message")).WithHeading("Message").WithMessage("I'm sorry to hear that. Let us know if we can do something to help you.").WithGroup(group1);
        var completedPart = new CompletedDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Completed")).WithHeading("Completed").WithMessage("Thank you for your input!").WithGroup(group2);
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .AddDecisions
            (
                new DecisionBuilder()
                    .AddConditions(new ConditionBuilder()
                        .WithLeftExpression(new GetDialogPartResultIdsByPartExpressionBuilder().WithDialogPartId(questionPart.Id))
                        .WithOperator(Operator.Contains)
                        .WithRightExpression(new ConstantExpressionBuilder().WithValue(answerTerrible.Id)))
                    .WithNextPartId(messagePart.Id)
            ).WithDefaultNextPartId(completedPart.Id);

        var parts = new DialogPartBuilder[]
        {
            new DialogPartBuilder(welcomePart),
            new DialogPartBuilder(questionPart),
            new DialogPartBuilder(decisionPart),
            new DialogPartBuilder(messagePart)
        }.Where(_ => addParts);
        return new DialogBuilder()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("HowDoYouFeel"))
            .AddParts(parts)
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1, group2);
    }
}
