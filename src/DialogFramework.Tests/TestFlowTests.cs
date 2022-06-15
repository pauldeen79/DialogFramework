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
            .AddSingleton<IDialogDefinitionRepository, TestDialogDefinitionRepository>()
            .AddSingleton(loggerMock.Object)
            .BuildServiceProvider();
        var dialogDefinition = provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(TestFlowDialog), "1.0.0"))!;
        var sut = provider.GetRequiredService<IDialogApplicationService>();

        // Act & Assert
        var dialog = sut.Start(dialogDefinition.Metadata).Value!; // empty -> Welcome
        dialog.CurrentPartId.Value.Should().Be("Welcome");
        dialog = sut.Continue(dialog).Value!; // Welcome -> How old are you?
        dialog.CurrentPartId.Value.Should().Be("Age");
        var result = sut.Continue(dialog); // How old are you -> empty answer -> validation error
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(Abstractions.Results.ResultStatus.Invalid);
        dialog.CurrentPartId.Value.Should().Be("Age");
        dialog = sut.Continue(dialog, new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("10-19"))
            .Build()).Value!; // How old are you -> 10-19 -> decision -> sports types
        dialog.CurrentPartId.Value.Should().Be("SportsTypes");
        dialog = sut.Continue(dialog).Value!; // Sports types -> empty answer -> unhealthy
        dialog.CurrentPartId.Value.Should().Be("Unhealthy");
        dialog = sut.Continue(dialog).Value!; // Unhealthy -> e-mail address
        dialog.CurrentPartId.Value.Should().Be("Email");
        dialog = sut.Continue(dialog).Value!; // E-mail address -> empty answer -> completed
        dialog.CurrentPartId.Value.Should().Be("Completed");
    }
}
