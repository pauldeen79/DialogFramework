﻿namespace DialogFramework.Domain.TestData;

public static class SimpleFormFlowDialog
{
    public static IDialog Create()
    {
        var getInformationGroupBuider = new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Get information"))
            .WithNumber(1)
            .WithTitle("Get information");
        var completedGroupBuilder = new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Completed"))
            .WithNumber(2)
            .WithTitle("Completed");
        return new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("SimpleFormFlowDialog")
                .WithFriendlyName("Simple form flow dialog")
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
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                        .WithHeading("Contact information")
                        .WithGroup(getInformationGroupBuider)
                        .WithTitle("Please provide your e-mail address and telephone number, so we can contact you.")
                        .AddResults
                        (
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                                .WithTitle("E-mail address")
                                .WithValueType(ResultValueType.Text)
                                .AddValidators
                                (
                                    new DialogPartResultDefinitionValidatorBuilder(new ValueTypeValidator(typeof(string))),
                                    new DialogPartResultDefinitionValidatorBuilder(new RequiredValidator(true))
                                ),
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                                .WithTitle("Telephone number")
                                .WithValueType(ResultValueType.Text)
                                .AddValidators
                                (
                                    new DialogPartResultDefinitionValidatorBuilder(new ValueTypeValidator(typeof(string))),
                                    new DialogPartResultDefinitionValidatorBuilder(new RequiredValidator(true))
                                )
                        )
                ),
                new DialogPartBuilder
                (
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                        .WithHeading("Newsletter")
                        .WithGroup(getInformationGroupBuider)
                        .WithTitle("Would you like to receive our newsletter?")
                        .AddResults
                        (
                            new DialogPartResultDefinitionBuilder()
                                .WithId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                                .WithTitle("Subscribe to newsletter (optional)")
                                .WithValueType(ResultValueType.YesNo)
                                .AddValidators
                                (
                                    new DialogPartResultDefinitionValidatorBuilder(new ValueTypeValidator(typeof(bool)))
                                )
                        )
                )
            )
            .AddPartGroups(getInformationGroupBuider, completedGroupBuilder)
            .Build();
    }
}