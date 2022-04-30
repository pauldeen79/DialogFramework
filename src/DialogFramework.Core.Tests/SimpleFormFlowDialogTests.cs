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

    [Fact]
    public void Providing_Wrong_ValueTypes_Leads_To_ValidationErrors()
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
                new NumberDialogPartResultValue(911)
            ),
            new DialogPartResult
            (
                context.CurrentPart.Id,
                "TelephoneNumber",
                new YesNoDialogPartResultValue(true)
            ),
            new DialogPartResult
            (
                context.CurrentPart.Id,
                "SignUpForNewsletter",
                new TextDialogPartResultValue("yes please")
            )
        ); // ContactInfo -> Completed
        context.CurrentPart.Id.Should().Be("ContactInfo");
        context.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var questionDialogPart = (IQuestionDialogPart)context.CurrentPart;
        questionDialogPart.ErrorMessages.Should().BeEquivalentTo(new[]
        {
            "Result for [ContactInfo.EmailAddress] should be of type [Text], but type [Number] was answered",
            "Result value of [ContactInfo.EmailAddress] is not of type [System.String]",
            "Result for [ContactInfo.TelephoneNumber] should be of type [Text], but type [YesNo] was answered",
            "Result value of [ContactInfo.TelephoneNumber] is not of type [System.String]",
            "Result for [ContactInfo.SignUpForNewsletter] should be of type [YesNo], but type [Text] was answered",
            "Result value of [ContactInfo.SignUpForNewsletter] is not of type [System.Boolean]"
        });
    }
}
