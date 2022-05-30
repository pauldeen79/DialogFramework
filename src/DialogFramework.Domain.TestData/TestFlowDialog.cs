namespace DialogFramework.Domain.TestData;

public static class TestFlowDialog
{
    public static IDialog Create()
    {
        var welcomeGroupBuilder = new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Welcome"))
            .WithNumber(1)
            .WithTitle("Welcome");
        var getInformationGroupBuider = new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Get information"))
            .WithNumber(2)
            .WithTitle("Get information");
        var completedGroupBuilder = new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Completed"))
            .WithNumber(3)
            .WithTitle("Completed");
        return new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("TestFlowDialog")
                .WithFriendlyName("Test flow dialog")
                .WithVersion("1.0.0"))
            .WithAbortedPart(new AbortedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Aborted"))
                .WithMessage("The dialog is aborted. You can come back any time to start the application again."))
            .WithErrorPart(new ErrorDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Error"))
                .WithErrorMessage("Something went wrong. Please try again, or contact us in case the problem persists."))
            .WithCompletedPart(new CompletedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Completed"))
                .WithHeading("Completed")
                .WithMessage("Thank you for using this application. Please come back soon!")
                .WithGroup(completedGroupBuilder))
            .AddParts
            (
                new DialogPartBuilder
                (
                    new MessageDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
                        .WithHeading("Welcome")
                        .WithGroup(welcomeGroupBuilder)
                        .WithMessage("Welcome to the health advisor application. By answering questions, we can give you an advice how to improve your health. You can continue to start analyzing your health.")
                ),
                new DialogPartBuilder
                (
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Age"))
                        .WithHeading("Age")
                        .WithGroup(getInformationGroupBuider)
                        .WithTitle("How old are you?")
                        .AddResults
                        (
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("<10"))
                                .WithTitle("0 to 9 years old"),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("10-19"))
                                .WithTitle("10 to 19 years old"),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("20-29"))
                                .WithTitle("20 to 29 years old"),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("30-39"))
                                .WithTitle("30 to 39 years old"),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("40-49"))
                                .WithTitle("40 to 49 years old"),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("50+"))
                                .WithTitle("Older than 50 years")
                        )
                        .AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator()))
                ),
                new DialogPartBuilder
                (
                    new DecisionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("AgeDecision"))
                        .AddDecisions
                        (
                            new DecisionBuilder()
                                .AddConditions
                                (
                                    new ConditionBuilder()
                                        .WithLeftExpression
                                        (
                                            new GetDialogPartResultIdsByPartExpressionBuilder()
                                                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Age"))
                                                .WithFunction(new ContainsFunctionBuilder().WithObjectToContain("<10"))
                                        )
                                        .WithOperator(Operator.Equal)
                                        .WithRightExpression(new ConstantExpressionBuilder().WithValue(true))
                                ).WithNextPartId(new DialogPartIdentifierBuilder().WithValue("TooYoung"))
                        ).WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("SportsTypes"))
                ),
                new DialogPartBuilder
                (
                    new MessageDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("TooYoung"))
                        .WithHeading("Completed")
                        .WithGroup(completedGroupBuilder)
                        .WithMessage("Too bad, you are too young. We can't give advice on kids.")
                ),
                new DialogPartBuilder
                (
                    new NavigationDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("TooYoungNavigation"))
                        .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email"))
                ),
                new DialogPartBuilder
                (
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("SportsTypes"))
                        .WithHeading("Sports types")
                        .WithGroup(getInformationGroupBuider)
                        .WithTitle("What type of sports do you do?")
                        .AddResults
                        (
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Bicycle")).WithTitle("Bicycle riding"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Soccer")).WithTitle("Soccer"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Swimming")).WithTitle("Swimming"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Aerobics")).WithTitle("Aerobics"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Tennis")).WithTitle("Tennis"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Baseball")).WithTitle("Baseball"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Hockey")).WithTitle("Hockey"),
                            new DialogPartResultDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Other")).WithTitle("Other sports (please specify)").WithValueType(ResultValueType.Text)
                        )
                ),
                new DialogPartBuilder
                (
                    new DecisionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("SportsTypeDecision"))
                        .AddDecisions
                        (
                            new DecisionBuilder()
                                .AddConditions
                                (
                                    new ConditionBuilder()
                                        .WithLeftExpression
                                        (
                                            new GetDialogPartResultValuesByPartExpressionBuilder()
                                                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("SportsTypes"))
                                                .WithFunction(new CountFunctionBuilder())
                                        )
                                        .WithOperator(Operator.Greater)
                                        .WithRightExpression(new ConstantExpressionBuilder().WithValue(0))
                                ).WithNextPartId(new DialogPartIdentifierBuilder().WithValue("Healthy"))
                        ).WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Unhealthy"))
                ),
                new DialogPartBuilder
                (
                    new MessageDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Healthy"))
                        .WithHeading("Healthy")
                        .WithGroup(completedGroupBuilder)
                        .WithMessage("You're all good! Keep up the good work.")
                ),
                new DialogPartBuilder
                (
                    new NavigationDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("HealthyNavigation"))
                        .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email"))
                ),
                new DialogPartBuilder
                (
                    new MessageDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Unhealthy"))
                        .WithHeading("Unhealthy")
                        .WithGroup(completedGroupBuilder)
                        .WithMessage("Our advice: It's time to do some sports, mate!")
                ),
                new DialogPartBuilder
                (
                    new NavigationDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("UnhealthyNavigation"))
                        .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email"))
                ),
                new DialogPartBuilder
                (
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Email"))
                        .WithHeading("E-mail address")
                        .WithGroup(completedGroupBuilder)
                        .WithTitle("Thank you for using this application. You can leave your e-mail address in case you have comments or questions.")
                        .AddResults
                        (
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                                .WithTitle("E-mail address")
                                .WithValueType(ResultValueType.Text)
                        )
                )
            )
            .AddPartGroups(welcomeGroupBuilder, getInformationGroupBuider, completedGroupBuilder)
            .Build();
    }
}
