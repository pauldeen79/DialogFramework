namespace DialogFramework.Tests;

public sealed class TestFlowTests : TestBase
{
    [Fact]
    public async Task Can_Complete_TestFlow_Dialog_With_Unhealthy_Advice()
    {
        // Act & Assert
        var dialog = (await StartHandler.Handle(new StartRequest(TestFlowDialogDefinition.Metadata), CancellationToken.None)).GetValueOrThrow("Start failed"); // empty -> Welcome
        dialog.CurrentPartId.Value.Should().Be("Welcome");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)).GetValueOrThrow("Welcome failed"); // Welcome -> How old are you?
        dialog.CurrentPartId.Value.Should().Be("Age");
        var result = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)); // How old are you -> empty answer -> validation error
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        dialog.CurrentPartId.Value.Should().Be("Age");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(dialog, new[] { new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("10-19"))
            .Build() }), CancellationToken.None)).GetValueOrThrow("How old are you failed"); // How old are you -> 10-19 -> decision -> sports types
        dialog.CurrentPartId.Value.Should().Be("SportsTypes");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)).GetValueOrThrow("Sports types failed"); // Sports types -> empty answer -> unhealthy
        dialog.CurrentPartId.Value.Should().Be("Unhealthy");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)).GetValueOrThrow("Unhealthy failed"); // Unhealthy -> e-mail address
        dialog.CurrentPartId.Value.Should().Be("Email");
        dialog = (await ContinueHandler.Handle(new ContinueRequest(dialog), CancellationToken.None)).GetValueOrThrow("E-mail address failed"); // E-mail address -> empty answer -> completed
        dialog.CurrentPartId.Value.Should().Be("Completed");
    }
}
