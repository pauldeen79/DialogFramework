namespace DialogFramework.Core.Tests;

public class SimpleFormFlowDialogTests
{
    [Fact]
    public void Can_Complete_SimpleFormFlow_Dialog()
    {
        // Arrange
        var dialog = new SimpleFormFlowDialog();
        var factory = new DialogContextFactory();
        var sut = new DialogService(factory);

        // Act & Assert
        var context = sut.Start(dialog);
        context.CurrentPart.Id.Should().Be("ContactInfo");
        context = sut.Continue
        (
            context,
            new DialogPartResult
            (
                context.CurrentPart.Id,
                "EmailAddress",
                new TextDialogPartResultValue("email@address.com")
            ),
            new DialogPartResult
            (
                context.CurrentPart.Id,
                "TelephoneNumber",
                new TextDialogPartResultValue("911")
            ),
            new DialogPartResult
            (
                context.CurrentPart.Id,
                "SignUpForNewsletter",
                new YesNoDialogPartResultValue(false)
            )
        ); // ContactInfo -> Completed
        context.CurrentPart.Id.Should().Be("Completed");
    }
}
