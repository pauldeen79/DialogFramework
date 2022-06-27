namespace DialogFramework.Core.Tests;

public class TestFlowTests
{
    [Fact]
    public void Can_Complete_TestFlow_Dialog_With_Unhealthy_Advice()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        using var provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton<IDialogDefinitionProvider, TestDialogDefinitionProvider>()
            .AddSingleton(loggerMock.Object)
            .BuildServiceProvider();
        var dialogDefinition = provider.GetRequiredService<IDialogDefinitionProvider>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(TestFlowDialog), "1.0.0")).GetValueOrThrow();
        var sut = provider.GetRequiredService<IDialogApplicationService>();

        // Act & Assert
        var dialog = sut.Start(dialogDefinition.Metadata).GetValueOrThrow("Start failed"); // empty -> Welcome
        dialog.CurrentPartId.Value.Should().Be("Welcome");
        dialog = sut.Continue(dialog).GetValueOrThrow("Welcome failed"); // Welcome -> How old are you?
        dialog.CurrentPartId.Value.Should().Be("Age");
        var result = sut.Continue(dialog); // How old are you -> empty answer -> validation error
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        dialog.CurrentPartId.Value.Should().Be("Age");
        dialog = sut.Continue(dialog, new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("10-19"))
            .Build()).GetValueOrThrow("How old are you failed"); // How old are you -> 10-19 -> decision -> sports types
        dialog.CurrentPartId.Value.Should().Be("SportsTypes");
        dialog = sut.Continue(dialog).GetValueOrThrow("Sports types failed"); // Sports types -> empty answer -> unhealthy
        dialog.CurrentPartId.Value.Should().Be("Unhealthy");
        dialog = sut.Continue(dialog).GetValueOrThrow("Unhealthy failed"); // Unhealthy -> e-mail address
        dialog.CurrentPartId.Value.Should().Be("Email");
        dialog = sut.Continue(dialog).GetValueOrThrow("E-mail address failed"); // E-mail address -> empty answer -> completed
        dialog.CurrentPartId.Value.Should().Be("Completed");
    }
}
