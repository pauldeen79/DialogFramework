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
        var dialog = provider.GetRequiredService<IDialogDefinitionRepository>().GetDialog(new DialogDefinitionIdentifier(nameof(TestFlowDialog), "1.0.0"))!;
        var sut = provider.GetRequiredService<IDialogService>();

        // Act & Assert
        var context = sut.Start(dialog.Metadata); // empty -> Welcome
        context.CurrentPartId.Value.Should().Be("Welcome");
        context = sut.Continue(context); // Welcome -> How old are you?
        context.CurrentPartId.Value.Should().Be("Age");
        context = sut.Continue(context); // How old are you -> empty answer -> validation error
        context.CurrentPartId.Value.Should().Be("Age");
        context = sut.Continue(context, new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("10-19"))
            .Build()); // How old are you -> 10-19 -> decision -> sports types
        context.CurrentPartId.Value.Should().Be("SportsTypes");
        context = sut.Continue(context); // Sports types -> empty answer -> unhealthy
        context.CurrentPartId.Value.Should().Be("Unhealthy");
        context = sut.Continue(context); // Unhealthy -> e-mail address
        context.CurrentPartId.Value.Should().Be("Email");
        context = sut.Continue(context); // E-mail address -> empty answer -> completed
        context.CurrentPartId.Value.Should().Be("Completed");
    }
}
