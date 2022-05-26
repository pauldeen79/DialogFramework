namespace DialogFramework.Core.Tests.DomainModel.Builders;

public class DialogPartBuilderTests
{
    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Question_DialogPart_Type()
    {
        // Arrange
        var input = new QuestionDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Aborted_DialogPart_Type()
    {
        // Arrange
        var input = new AbortedDialogPartBuilder().WithId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Error_DialogPart_Type()
    {
        // Arrange
        var input = new ErrorDialogPartBuilder().WithId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Completed_DialogPart_Type()
    {
        // Arrange
        var input = new CompletedDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Message_DialogPart_Type()
    {
        // Arrange
        var input = new MessageDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Decision_DialogPart_Type()
    {
        // Arrange
        var input = new DecisionDialogPartBuilder().WithId("Test").WithDefaultNextPartId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Navigation_DialogPart_Type()
    {
        // Arrange
        var input = new NavigationDialogPartBuilder().WithId("Test").WithNavigateToId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Redirect_DialogPart_Type()
    {
        // Arrange
        var input = new RedirectDialogPartBuilder().WithId("Test").WithRedirectDialogMetadata(new DialogMetadataBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input);

        // Assert
        sut.Should().BeOfType<DialogPartBuilder>();
    }
}
