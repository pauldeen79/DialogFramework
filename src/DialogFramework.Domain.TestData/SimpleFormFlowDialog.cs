﻿namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class SimpleFormFlowDialog
{
    public static IDialogDefinition Create()
        => DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("SimpleFormFlowDialog")
                .WithFriendlyName("Simple form flow dialog")
                .WithVersion("1.0.0"))
            .AddParts
            (
                new QuestionDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                    .WithHeading("Contact information")
                    .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
                    .WithTitle("Please provide your e-mail address and telephone number, so we can contact you.")
                    .AddAnswers
                    (
                        new DialogPartResultAnswerDefinitionBuilder()
                            .WithId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                            .WithTitle("E-mail address")
                            .WithValueType(ResultValueType.Text)
                            .AddValidators
                            (
                                new DialogPartResultAnswerDefinitionValidatorBuilder(new ValueTypeValidator(typeof(string))),
                                new DialogPartResultAnswerDefinitionValidatorBuilder(new RequiredValidator(true))
                            ),
                        new DialogPartResultAnswerDefinitionBuilder()
                            .WithId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                            .WithTitle("Telephone number")
                            .WithValueType(ResultValueType.Text)
                            .AddValidators
                            (
                                new DialogPartResultAnswerDefinitionValidatorBuilder(new ValueTypeValidator(typeof(string))),
                                new DialogPartResultAnswerDefinitionValidatorBuilder(new RequiredValidator(true))
                            )
                    ),
                new QuestionDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                    .WithHeading("Newsletter")
                    .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
                    .WithTitle("Would you like to receive our newsletter?")
                    .AddAnswers
                    (
                        new DialogPartResultAnswerDefinitionBuilder()
                            .WithId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                            .WithTitle("Subscribe to newsletter (optional)")
                            .WithValueType(ResultValueType.YesNo)
                            .AddValidators
                            (
                                new DialogPartResultAnswerDefinitionValidatorBuilder(new ValueTypeValidator(typeof(bool)))
                            )
                    )
            )
            .AddPartGroups(DialogPartGroupFixture.CreateGetInformationGroupBuider(), DialogPartGroupFixture.CreateCompletedGroupBuilder())
            .Build();
}
