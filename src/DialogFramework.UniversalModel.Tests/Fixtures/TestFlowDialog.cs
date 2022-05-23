﻿using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.Domains;
using DialogFramework.Core.DomainModel.QuestionDialogPartValidators;
using DialogFramework.UniversalModel.DomainModel.Builders;
using DialogFramework.UniversalModel.DomainModel.DialogParts.Builders;

namespace DialogFramework.UniversalModel.Tests.Fixtures
{
    public static class TestFlowDialog
    {
        public static IDialog Create()
        {
            var welcomeGroupBuilder = new DialogPartGroupBuilder().WithId("Welcome").WithNumber(1).WithTitle("Welcome");
            var getInformationGroupBuider = new DialogPartGroupBuilder().WithId("Get information").WithNumber(2).WithTitle("Get information");
            var completedGroupBuilder = new DialogPartGroupBuilder().WithId("Completed").WithNumber(3).WithTitle("Completed");
            return new DialogBuilder()
                .WithMetadata(new DialogMetadataBuilder().WithId("TestFlowDialog").WithFriendlyName("Test flow dialog").WithVersion("1.0.0"))
                .WithAbortedPart(new AbortedDialogPartBuilder().WithId("Aborted").WithMessage("The dialog is aborted. You can come back any time to start the application again."))
                .WithErrorPart(new ErrorDialogPartBuilder().WithId("Error").WithErrorMessage("Something went wrong. Please try again, or contact us in case the problem persists."))
                .WithCompletedPart(new CompletedDialogPartBuilder().WithId("Completed").WithHeading("Completed").WithMessage("Thank you for using this application. Please come back soon!").WithGroup(completedGroupBuilder))
                .AddParts
                (
                    new DialogPartBuilder
                    (
                        new MessageDialogPartBuilder()
                            .WithId("Welcome")
                            .WithHeading("Welcome")
                            .WithGroup(welcomeGroupBuilder)
                            .WithMessage("Welcome to the health advisor application. By answering questions, we can give you an advice how to improve your health. You can continue to start analyzing your health.")
                    ),
                    new DialogPartBuilder
                    (
                        new QuestionDialogPartBuilder()
                            .WithId("Age")
                            .WithHeading("Age")
                            .WithGroup(getInformationGroupBuider)
                            .WithTitle("How old are you?")
                            .AddResults
                            (
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("<10")
                                    .WithTitle("0 to 9 years old"),
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("10-19")
                                    .WithTitle("10 to 19 years old"),
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("20-29")
                                    .WithTitle("20 to 29 years old"),
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("30-39")
                                    .WithTitle("30 to 39 years old"),
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("40-49")
                                    .WithTitle("40 to 49 years old"),
                                new DialogPartResultDefinitionBuilder()
                                    .WithId("50+")
                                    .WithTitle("Older than 50 years")
                            )
                            .AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator()))
                    ),
                    new DialogPartBuilder
                    (
                        new DecisionDialogPartBuilder()
                            .WithId("AgeDecision") //TODO: Add decision rules here. <10 goes to TooYoung. other values go to SportTypes
                    ),
                    new DialogPartBuilder
                    (
                        new MessageDialogPartBuilder()
                            .WithId("TooYoung")
                            .WithHeading("Completed")
                            .WithGroup(completedGroupBuilder)
                            .WithMessage("Too bad, you are too young. We can't give advice on kids.")
                    ),
                    new DialogPartBuilder
                    (
                        new NavigationDialogPartBuilder()
                            .WithId("TooYoungNavigation")
                            .WithNavigateToId("EmailPart")
                    ),
                    new DialogPartBuilder
                    (
                        new QuestionDialogPartBuilder()
                            .WithId("SportsTypes")
                            .WithHeading("Sports types")
                            .WithGroup(getInformationGroupBuider)
                            .WithTitle("What type of sports do you do?")
                            .AddResults
                            (
                                new DialogPartResultDefinitionBuilder().WithId("Bicycle").WithTitle("Bicycle riding"),
                                new DialogPartResultDefinitionBuilder().WithId("Soccer").WithTitle("Soccer"),
                                new DialogPartResultDefinitionBuilder().WithId("Swimming").WithTitle("Swimming"),
                                new DialogPartResultDefinitionBuilder().WithId("Aerobics").WithTitle("Aerobics"),
                                new DialogPartResultDefinitionBuilder().WithId("Tennis").WithTitle("Tennis"),
                                new DialogPartResultDefinitionBuilder().WithId("Baseball").WithTitle("Baseball"),
                                new DialogPartResultDefinitionBuilder().WithId("Hockey").WithTitle("Hockey"),
                                new DialogPartResultDefinitionBuilder().WithId("Other").WithTitle("Other sports (please specify)").WithValueType(ResultValueType.Text)
                            )
                    ),
                    new DialogPartBuilder
                    (
                        new DecisionDialogPartBuilder()
                            .WithId("SportsTypeDecision") //TODO: Add decision rules here. any value goes to Healthy. no values goes to Unhealthy
                    ),
                    new DialogPartBuilder
                    (
                        new MessageDialogPartBuilder()
                            .WithId("Healthy")
                            .WithHeading("Healthy")
                            .WithGroup(completedGroupBuilder)
                            .WithMessage("You're all good! Keep up the good work.")
                    ),
                    new DialogPartBuilder
                    (
                        new NavigationDialogPartBuilder()
                            .WithId("HealthyNavigation")
                            .WithNavigateToId("EmailPart")
                    ),
                    new DialogPartBuilder
                    (
                        new MessageDialogPartBuilder()
                            .WithId("Unhealthy")
                            .WithHeading("Unhealthy")
                            .WithGroup(completedGroupBuilder)
                            .WithMessage("Our advice: It's time to do some sports, mate!")
                    ),
                    new DialogPartBuilder
                    (
                        new NavigationDialogPartBuilder()
                            .WithId("UnhealthyNavigation")
                            .WithNavigateToId("EmailPart")
                    ),
                    new DialogPartBuilder
                    (
                        new QuestionDialogPartBuilder()
                            .WithId("Email")
                            .WithHeading("E-mail address")
                            .WithGroup(completedGroupBuilder)
                            .WithTitle("Thank you for using this application. You can leave your e-mail address in case you have comments or questions.")
                            .AddResults
                            (
                                new DialogPartResultDefinitionBuilder().WithId("EmailAddress").WithTitle("E-mail address").WithValueType(ResultValueType.Text)
                            )
                    )
                )
                .AddPartGroups(welcomeGroupBuilder, getInformationGroupBuider, completedGroupBuilder)
                .Build();
        }
    }
}
