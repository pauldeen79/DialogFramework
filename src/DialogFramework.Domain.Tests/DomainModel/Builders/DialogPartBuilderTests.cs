﻿namespace DialogFramework.Domain.Tests.DomainModel.Builders;

public class DialogPartBuilderTests
{
    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Question_DialogPart_Type()
    {
        // Arrange
        var input = new QuestionDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IQuestionDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Aborted_DialogPart_Type()
    {
        // Arrange
        var input = new AbortedDialogPartBuilder().WithId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IAbortedDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Error_DialogPart_Type()
    {
        // Arrange
        var input = new ErrorDialogPartBuilder().WithId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IErrorDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Completed_DialogPart_Type()
    {
        // Arrange
        var input = new CompletedDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<ICompletedDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Message_DialogPart_Type()
    {
        // Arrange
        var input = new MessageDialogPartBuilder().WithId("Test").WithGroup(new DialogPartGroupBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IMessageDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Decision_DialogPart_Type()
    {
        // Arrange
        var input = new DecisionDialogPartBuilder().WithId("Test").WithDefaultNextPartId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IDecisionDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Navigation_DialogPart_Type()
    {
        // Arrange
        var input = new NavigationDialogPartBuilder().WithId("Test").WithNavigateToId("Test").Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<INavigationDialogPart>();
    }

    [Fact]
    public void Can_Construct_DialogPartBuilder_From_Redirect_DialogPart_Type()
    {
        // Arrange
        var input = new RedirectDialogPartBuilder().WithId("Test").WithRedirectDialogMetadata(new DialogMetadataBuilder()).Build();

        // Act
        var sut = new DialogPartBuilder(input).Build();

        // Assert
        sut.Should().BeAssignableTo<IRedirectDialogPart>();
    }

    [Fact]
    public void Unknown_DialogPart_Type_Throws_On_Construction()
    {
        // Arrange
        var unknownDialogType = new Mock<IDialogPart>().Object;
        var constructingUnknownType = new Action(() => _ = new DialogPartBuilder(unknownDialogType));

        // Act & Assert
        constructingUnknownType.Should().Throw<ArgumentException>().WithMessage("Dialogpart type [Castle.Proxies.IDialogPartProxy] is not supported");
    }
}