namespace DialogFramework.Core.Tests;

public sealed class SimpleFormFlowDialogTests : IDisposable
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly ServiceProvider _provider;

    public SimpleFormFlowDialogTests()
    {
        _loggerMock = new Mock<ILogger>();
        _provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton<IDialogDefinitionRepository, TestDialogDefinitionRepository>()
            .AddSingleton(_loggerMock.Object)
            .BuildServiceProvider();
    }

    [Fact]
    public void Can_Complete_SimpleFormFlow_Dialog_In_One_Step()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act
        var dialog = sut.Start(dialogDefinition.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ).Value!; // ContactInfo -> Newsletter
        dialog = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ).Value!; // Newsletter -> Completed

        // Assert
        dialog.CurrentState.Should().Be(DialogState.Completed);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("Completed");
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        });
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public void Can_Complete_SimpleFormFlow_Dialog_With_NavigateBack()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act
        var dialog = sut.Start(dialogDefinition.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("wrong@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ).Value!; // ContactInfo -> Newsletter
        dialog = sut.NavigateTo(dialog, new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Value!; // navigate back: Completed -> ContactInfo
        dialog = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ).Value!; // ContactInfo -> Newsletter
        dialog = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ).Value!; // Newsletter -> Completed

        // Assert
        dialog.CurrentState.Should().Be(DialogState.Completed);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("Completed");
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        });
        dialog.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public void Can_Complete_SimpleFormFlow_In_Different_Session()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act step 1: Start a session, submit first question
        var dialog1 = sut.Start(dialogDefinition.Metadata).Value!;
        dialog1.CurrentPartId.Value.Should().Be("ContactInfo");
        dialog1 = sut.Continue
        (
            dialog1,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog1.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog1.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ).Value!; // ContactInfo -> Newsletter

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogBuilder(dialog1));

        // Act step 2: Re-create the dialog in a new session (simulating that the dialog is saved to a store, and reconstructed again)
        var dialog2 = JsonSerializerFixture.Deserialize<DialogBuilder>(json)!.Build();
        var result = sut.Continue
        (
            dialog2,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog2.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ).Value!; // Newsletter -> Completed

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        result.CurrentPartId.Value.Should().Be("Completed");
        result.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("ContactInfo"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        });
        result.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).Should().BeEquivalentTo(new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Newsletter"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        });
    }

    [Fact]
    public void Providing_Wrong_ValueTypes_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act
        var dialog = sut.Start(dialogDefinition!.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new NumberDialogPartResultValueBuilder().WithValue(911))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result for [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] should be of type [Text], but type [Number] was answered",
            "Result for [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] should be of type [Text], but type [YesNo] was answered",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is not of type [System.String]",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is not of type [System.String]"
        });
    }

    [Fact]
    public void Providing_Results_With_Empty_Values_On_Required_Values_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act
        var dialog = sut.Start(dialogDefinition!.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue(string.Empty))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue((object?)null))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

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
    public void Providing_Results_With_No_Values_On_Required_Values_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogApplicationService>();

        // Act
        var dialog = sut.Start(dialogDefinition!.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = sut.Continue(dialog); // Current part remains ContactInfo because of validation errors

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
    public void Providing_Results_With_Wrong_ValueType_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialogDefinition = new TestDialogDefinitionRepository().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"));
        var factory = new DialogFactory();
        var repository = new TestDialogDefinitionRepository();
        var sut = new DialogApplicationService(factory, repository, new Mock<IConditionEvaluator>().Object, new Mock<ILogger>().Object);

        // Act
        var dialog = sut.Start(dialogDefinition!.Metadata).Value!;
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        var result = sut.Continue
        (
            dialog,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new NumberDialogPartResultValueBuilder().WithValue(1))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

        // Assert
        dialog.CurrentState.Should().Be(DialogState.InProgress);
        dialog.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        dialog.CurrentPartId.Value.Should().Be("ContactInfo");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result for [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] should be of type [Text], but type [Number] was answered",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is not of type [System.String]"
        });
    }

    public void Dispose() => _provider.Dispose();
}
