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
            .AddSingleton<IDialogRepository, TestDialogRepository>()
            .AddSingleton(loggerMock.Object)
            .BuildServiceProvider();
        var dialog = provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(TestFlowDialog), "1.0.0"))!;
        var sut = provider.GetRequiredService<IDialogService>();

        // Act & Assert
        var context = sut.Start(dialog.Metadata); // empty -> Welcome
        context.CurrentPartId.Should().Be("Welcome");
        context = sut.Continue(context); // Welcome -> How old are you?
        context.CurrentPartId.Should().Be("Age");
        context = sut.Continue(context); // How old are you -> empty answer -> validation error
        context.CurrentPartId.Should().Be("Age");
        context = sut.Continue(context, new DialogPartResultBuilder()
            .WithDialogPartId(context.CurrentPartId)
            .WithResultId("10-19")
            .Build()); // How old are you -> 10-19 -> decision -> sports types
        context.CurrentPartId.Should().Be("SportsTypes");
        context = sut.Continue(context); // Sports types -> empty answer -> unhealthy
        context.CurrentPartId.Should().Be("Unhealthy");
        context = sut.Continue(context); // Unhealthy -> e-mail address
        context.CurrentPartId.Should().Be("Email");
        context = sut.Continue(context); // E-mail address -> empty answer -> completed
        context.CurrentPartId.Should().Be("Completed");
    }
}
