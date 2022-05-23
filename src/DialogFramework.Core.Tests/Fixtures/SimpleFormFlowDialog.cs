﻿namespace DialogFramework.Core.Tests.Fixtures;

public record SimpleFormFlowDialog : Dialog
{
    private static readonly IDialogPartGroup GetInformationGroup = new DialogPartGroup("Get information", "Get information", 1);
    private static readonly IDialogPartGroup CompletedGroup = new DialogPartGroup("Completed", "Completed", 2);

    public SimpleFormFlowDialog() : base(
        new DialogMetadata(nameof(SimpleFormFlowDialog), "Simple fom flow dialog", "1.0.0", true),
        new IDialogPart[]
        {
            new QuestionDialogPart
            (
                "ContactInfo",
                "Contact information",
                "Please provide your e-mail address and telephone number, so we can contact you.",
                GetInformationGroup,
                new IDialogPartResultDefinition[]
                {
                    new DialogPartResultDefinition("EmailAddress", "E-mail address", ResultValueType.Text, new IDialogPartResultDefinitionValidator[] { new ValueTypeValidator(typeof(string)), new RequiredValidator(true) }),
                    new DialogPartResultDefinition("TelephoneNumber", "Telephone number", ResultValueType.Text, new IDialogPartResultDefinitionValidator[] { new ValueTypeValidator(typeof(string)), new RequiredValidator(true) })
                },
                Enumerable.Empty<IQuestionDialogPartValidator>()
            ),
            new QuestionDialogPart
            (
                "Newsletter",
                "Newsletter",
                "Would you like to receive our newsletter?",
                GetInformationGroup,
                new IDialogPartResultDefinition[]
                {
                    new DialogPartResultDefinition("SignUpForNewsletter", "Subscribe to newsletter (optional)", ResultValueType.YesNo, new[] { new ValueTypeValidator(typeof(bool)) })
                },
                Enumerable.Empty<IQuestionDialogPartValidator>()
            )
        },
        new ErrorDialogPart("Error", "Something went wrong. Please try again, or contact us in case the problem persists.", null),
        new AbortedDialogPart("Aborted", "The dialog is aborted. You can come back any time to start the application again."),
        new CompletedDialogPart("Completed", "Completed", "Thank you for using this application. Please come back soon!", CompletedGroup),
        new[] { GetInformationGroup, CompletedGroup }
        )
    {
    }
}
