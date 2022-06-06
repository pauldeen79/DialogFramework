namespace DialogFramework.Domain.TestData;

public static class DialogDefinitionFixture
{
    public static DialogDefinitionBuilder CreateBuilder()
        => CreateBuilderBase()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("DialogDefinitionFixture"))
            .AddParts
            (
                QuestionDialogPartFixture.CreateBuilder(),
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder()
                    .WithValue("Message"))
                    .WithHeading("Message")
                    .WithMessage("This is a message")
                    .WithGroup(DialogPartGroupFixture.CreateBuilder())
            )
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder());

    public static DialogDefinitionBuilder CreateHowDoYouFeelBuilder(bool addParts = true)
    {
        var group1 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part1")).WithTitle("Give information").WithNumber(1);
        var group2 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part2")).WithTitle("Completed").WithNumber(2);
        var welcomePart = new MessageDialogPartBuilder().WithHeading("Welcome").WithId(new DialogPartIdentifierBuilder().WithValue("Welcome")).WithMessage("Welcome! I would like to answer a question").WithGroup(group1);
        var answerGreat = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Great")).WithTitle("I feel great, thank you!");
        var answerOkay = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Okay")).WithTitle("I feel kind of okay");
        var answerTerrible = new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Terrible")).WithTitle("I feel terrible, don't want to talk about it");
        var questionPart = new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Question1"))
            .WithHeading("How do you feel")
            .WithTitle("Please tell us how you feel")
            .WithGroup(group1)
            .AddResults(answerGreat, answerOkay, answerTerrible)
            .AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator()));
        var messagePart = new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Message")).WithHeading("Message").WithMessage("I'm sorry to hear that. Let us know if we can do something to help you.").WithGroup(group1);
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
            ).WithDefaultNextPartId(CreateBuilderBase().CompletedPart.Id);

        var parts = new IDialogPartBuilder[]
        {
            welcomePart,
            questionPart,
            decisionPart,
            messagePart
        }.Where(_ => addParts);
        return CreateBuilderBase()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("HowDoYouFeel"))
            .AddParts(parts)
            .AddPartGroups(group1, group2);
    }

    public static DialogDefinitionBuilder CreateBuilderBase()
        => new DialogDefinitionBuilder()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("DialogDefinitionFixtureBase"))
            .WithAbortedPart(new AbortedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Abort"))
                .WithMessage("The dialog is aborted. You can come back any time to start the application again."))
            .WithErrorPart(new ErrorDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Error"))
                .WithErrorMessage("Something went wrong. Please try again, or contact us in case the problem persists."))
            .WithCompletedPart(new CompletedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Completed"))
                .WithHeading("Completed")
                .WithMessage("Thank you for using this application. Please come back soon!")
                .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder()));
}
