namespace DialogFramework.Tests;

public sealed class SimpleFormFlowDialogTests : TestBase
{
    [Fact]
    public async Task Can_Complete_SimpleFormFlow_Dialog_In_One_Step()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("ContactInfo failed"); // ContactInfo -> Newsletter
        dialog = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[] { new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                .Build() }
        ), CancellationToken.None)).GetValueOrThrow("Newsletter failed"); // Newsletter -> Completed

        // Assert
        dialog.CurrentState.Should().Be(DialogState.Completed);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("Completed");
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                .Build()
        });
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public async Task Can_Complete_SimpleFormFlow_Dialog_With_NavigateBack()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("wrong@address.com"))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("ContactInfo failed"); // ContactInfo -> Newsletter
        dialog = (await NavigateHandler.Handle(new NavigateRequest(dialog, new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()), CancellationToken.None)).GetValueOrThrow("Navigate back to ContactInfo failed"); // navigate back: Completed -> ContactInfo
        dialog = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("ContactInfo second time failed"); // ContactInfo -> Newsletter
        dialog = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("Newsletter failed"); // Newsletter -> Completed

        // Assert
        dialog.CurrentState.Should().Be(DialogState.Completed);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("Completed");
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogId(new DialogDefinitionIdentifierBuilder(SimpleFormFlowDialogDefinition.Metadata))
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogId(new DialogDefinitionIdentifierBuilder(SimpleFormFlowDialogDefinition.Metadata))
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                .Build()
        });
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogId(new DialogDefinitionIdentifierBuilder(SimpleFormFlowDialogDefinition.Metadata))
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public async Task Can_Complete_SimpleFormFlow_In_Different_Session()
    {
        // Act step 1: Start a session, submit first question
        var dialog1 = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog1.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog1 = (await ContinueHandler.Handle(new ContinueRequest(
            dialog1,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("ContactInfo failed"); // ContactInfo -> Newsletter

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogBuilder(dialog1, SimpleFormFlowDialogDefinition));

        // Act step 2: Re-create the dialog in a new session (simulating that the dialog is saved to a store, and reconstructed again)
        var dialog2 = JsonSerializerFixture.Deserialize<DialogBuilder>(json)!.Build();
        var result = (await ContinueHandler.Handle(new ContinueRequest(
            dialog2,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                    .Build()
            }
        ), CancellationToken.None)).GetValueOrThrow("Newsletter failed"); // Newsletter -> Completed

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        result.CurrentPartId.Value.Should().Be("Completed");
        result.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                .Build()
        });
        result.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).GetValueOrThrow().Should().BeEquivalentTo(new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public async Task Providing_Wrong_ValueTypes_Leads_To_ValidationErrors()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition!.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(911))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true))
                    .Build()
            }
        ), CancellationToken.None)); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is not of type [System.String]",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is not of type [System.String]"
        });
    }

    [Fact]
    public async Task Providing_Results_With_Empty_Values_On_Required_Values_Leads_To_ValidationErrors()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition!.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(string.Empty))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue((object?)null))
                    .Build()
            }
        ), CancellationToken.None)); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is required",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is required"
        });
    }

    [Fact]
    public async Task Providing_Results_With_No_Values_On_Required_Values_Leads_To_ValidationErrors()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition!.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is required",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is required"
        });
    }

    [Fact]
    public async Task Providing_Results_With_Wrong_ValueType_Leads_To_ValidationErrors()
    {
        // Act
        var dialog = (await StartHandler.Handle(new StartRequest(SimpleFormFlowDialogDefinition!.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed");
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = (await ContinueHandler.Handle(new ContinueRequest(
            dialog,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(1))
                    .Build(),
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue("911"))
                    .Build()
            }
        ), CancellationToken.None)); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is not of type [System.String]"
        });
    }
}
