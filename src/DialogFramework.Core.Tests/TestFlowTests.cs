namespace DialogFramework.Core.Tests;

public class TestFlowTests
{
    [Fact]
    public void Can_Complete_TestFlow_Dialog_With_Unhealthy_Advice()
    {
        // Arrange
        var dialog = new TestFlowDialog();
        var factory = new DialogContextFactory();
        var sut = new DialogService(factory);

        // Act & Assert
        var context = sut.Start(dialog); // empty -> Welcome
        context.CurrentPart.Id.Should().Be("Welcome");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart) }); // Welcome -> How old are you?
        context.CurrentPart.Id.Should().Be("Age");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart) }); // How old are you -> empty answer -> validation error
        context.CurrentPart.Id.Should().Be("Age");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart, new DialogPartResultDefinition("10-19", "", ResultValueType.None)) }); // How old are you -> 10-19 -> decision -> sports types
        context.CurrentPart.Id.Should().Be("SportsTypes");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart) }); // Sports types -> empty answer -> unhealthy
        context.CurrentPart.Id.Should().Be("Unhealthy");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart) }); // Unhealthy -> e-mail address
        context.CurrentPart.Id.Should().Be("Email");
        context = sut.Continue(context, new[] { new DialogPartResult(context.CurrentPart) }); // E-mail address -> empty answer -> completed
        context.CurrentPart.Id.Should().Be("Completed");
    }
}
