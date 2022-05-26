﻿namespace DialogFramework.Core.Tests;

public class TestFlowTests
{
    [Fact]
    public void Can_Complete_TestFlow_Dialog_With_Unhealthy_Advice()
    {
        // Arrange
        using var provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton<IDialogRepository, TestDialogRepository>()
            .BuildServiceProvider();
        var dialog = provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier(nameof(TestFlowDialog), "1.0.0"))!;
        var sut = provider.GetRequiredService<IDialogService>();

        // Act & Assert
        var context = sut.Start(dialog.Metadata); // empty -> Welcome
        context.CurrentPart.Id.Should().Be("Welcome");
        context = sut.Continue(context); // Welcome -> How old are you?
        context.CurrentPart.Id.Should().Be("Age");
        context = sut.Continue(context); // How old are you -> empty answer -> validation error
        context.CurrentPart.Id.Should().Be("Age");
        context = sut.Continue(context, new DialogPartResultBuilder()
            .WithDialogPartId(context.CurrentPart.Id)
            .WithResultId("10-19")
            .Build()); // How old are you -> 10-19 -> decision -> sports types
        context.CurrentPart.Id.Should().Be("SportsTypes");
        context = sut.Continue(context); // Sports types -> empty answer -> unhealthy
        context.CurrentPart.Id.Should().Be("Unhealthy");
        context = sut.Continue(context); // Unhealthy -> e-mail address
        context.CurrentPart.Id.Should().Be("Email");
        context = sut.Continue(context); // E-mail address -> empty answer -> completed
        context.CurrentPart.Id.Should().Be("Completed");
    }
}