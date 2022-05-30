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
            .AddSingleton<IDialogRepository, TestDialogRepository>()
            .AddSingleton(_loggerMock.Object)
            .BuildServiceProvider();
    }

    [Fact]
    public void Can_Complete_SimpleFormFlow_Dialog_In_One_Step()
    {
        // Arrange
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act
        var context = sut.Start(dialog.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // ContactInfo -> Newsletter
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ); // Newsletter -> Completed

        // Assert
        context.CurrentState.Should().Be(DialogState.Completed);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("Completed");
        context.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Should().BeEquivalentTo(new[]
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
        context.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).Should().BeEquivalentTo(new[]
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
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act
        var context = sut.Start(dialog.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("wrong@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // ContactInfo -> Newsletter
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
                .Build()
        ); // Newsletter -> Completed
        context = sut.NavigateTo(context, new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()); // navigate back: Completed -> ContactInfo
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // ContactInfo -> Newsletter
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ); // Newsletter -> Completed

        // Assert
        context.CurrentState.Should().Be(DialogState.Completed);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("Completed");
        context.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("ContactInfo").Build()).Should().BeEquivalentTo(new[]
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
        context.GetDialogPartResultsByPartIdentifier(new DialogPartIdentifierBuilder().WithValue("Newsletter").Build()).Should().BeEquivalentTo(new[]
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
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act step 1: Start a session, submit first question
        var context = sut.Start(dialog.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // ContactInfo -> Newsletter

        // Serialize
        var json = JsonSerializerFixture.Serialize(new DialogContextBuilder(context));

        // Act step 2: Re-create the context in a new session (simulating that the context is saved to a store, and reconstructed again)
        var context2 = JsonSerializerFixture.Deserialize<DialogContextBuilder>(json)!.Build();
        var result = sut.Continue
        (
            context2,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context2.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                .Build()
        ); // Newsletter -> Completed

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
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act
        var context = sut.Start(dialog!.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new NumberDialogPartResultValueBuilder().WithValue(911))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

        // Assert
        context.CurrentState.Should().Be(DialogState.InProgress);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
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
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act
        var context = sut.Start(dialog!.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue(string.Empty))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue((object?)null))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

        // Assert
        context.CurrentState.Should().Be(DialogState.InProgress);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is required",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is required"
        });
    }

    [Fact]
    public void Providing_Results_With_No_Values_On_Required_Values_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialog = _provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
        var sut = _provider.GetRequiredService<IDialogService>();

        // Act
        var context = sut.Start(dialog!.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue(context); // Current part remains ContactInfo because of validation errors

        // Assert
        context.CurrentState.Should().Be(DialogState.InProgress);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is required",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = TelephoneNumber }] is required"
        });
    }

    [Fact]
    public void Providing_Results_With_Wrong_ValueType_Leads_To_ValidationErrors()
    {
        // Arrange
        var dialog = new TestDialogRepository().GetDialog(new DialogIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"));
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository, new Mock<IConditionEvaluator>().Object, new Mock<ILogger>().Object);

        // Act
        var context = sut.Start(dialog!.Metadata);
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
                .WithValue(new NumberDialogPartResultValueBuilder().WithValue(1))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
                .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
                .Build()
        ); // Current part remains ContactInfo because of validation errors

        // Assert
        context.CurrentState.Should().Be(DialogState.InProgress);
        context.CurrentDialogIdentifier.Id.Should().Be(nameof(SimpleFormFlowDialog));
        context.CurrentPartId.Value.Should().Be("ContactInfo");
        context.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Result for [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] should be of type [Text], but type [Number] was answered",
            "Result value of [DialogPartIdentifier { Value = ContactInfo }.DialogPartResultIdentifier { Value = EmailAddress }] is not of type [System.String]"
        });
    }

    public void Dispose() => _provider.Dispose();
}
