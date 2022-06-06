namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class TestFlowDialog
{
    public static IDialogDefinition Create()
        => DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("TestFlowDialog")
                .WithFriendlyName("Test flow dialog")
                .WithVersion("1.0.0"))
            .AddParts
            (
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
                    .WithHeading("Welcome")
                    .WithGroup(DialogPartGroupFixture.CreateWelcomeGroupBuilder())
                    .WithMessage("Welcome to the health advisor application. By answering questions, we can give you an advice how to improve your health. You can continue to start analyzing your health."),
                new QuestionDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Age"))
                    .WithHeading("Age")
                    .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
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
                    .AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())),
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
                    ).WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("SportsTypes")),
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("TooYoung"))
                    .WithHeading("Completed")
                    .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder())
                    .WithMessage("Too bad, you are too young. We can't give advice on kids."),
                new NavigationDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("TooYoungNavigation"))
                    .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email")),
                new QuestionDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("SportsTypes"))
                    .WithHeading("Sports types")
                    .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
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
                    ),
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
                    ).WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Unhealthy")),
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Healthy"))
                    .WithHeading("Healthy")
                    .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder())
                    .WithMessage("You're all good! Keep up the good work."),
                new NavigationDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("HealthyNavigation"))
                    .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email")),
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Unhealthy"))
                    .WithHeading("Unhealthy")
                    .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder())
                    .WithMessage("Our advice: It's time to do some sports, mate!"),
                new NavigationDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("UnhealthyNavigation"))
                    .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Email")),
                new QuestionDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Email"))
                    .WithHeading("E-mail address")
                    .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder())
                    .WithTitle("Thank you for using this application. You can leave your e-mail address in case you have comments or questions.")
                    .AddResults
                    (
                        new DialogPartResultDefinitionBuilder()
                            .WithId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                            .WithTitle("E-mail address")
                            .WithValueType(ResultValueType.Text)
                    )
            )
            .AddPartGroups(DialogPartGroupFixture.CreateWelcomeGroupBuilder(), DialogPartGroupFixture.CreateGetInformationGroupBuider(), DialogPartGroupFixture.CreateCompletedGroupBuilder())
            .Build();
}
