namespace DialogFramework.Domain.TestData;

public static class SimpleFormFlowDialog
{
    public static IDialog Create()
        => DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("SimpleFormFlowDialog")
                .WithFriendlyName("Simple form flow dialog")
                .WithVersion("1.0.0"))
            .AddParts
            (
                new DialogPartBuilder
                (
                    new QuestionDialogPartBuilder()
                        .WithId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                        .WithHeading("Contact information")
                        .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
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
                        .WithGroup(DialogPartGroupFixture.CreateGetInformationGroupBuider())
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
            .AddPartGroups(DialogPartGroupFixture.CreateGetInformationGroupBuider(), DialogPartGroupFixture.CreateCompletedGroupBuilder())
            .Build();
}
