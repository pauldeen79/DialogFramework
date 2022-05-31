namespace DialogFramework.Domain.Tests.DialogParts;

public class MessageDialogPartTests
{
    [Fact]
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new MessageDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .Build();

        // Act
        var actual = input.CreateBuilder();

        // Assert
        actual.Build().Should().BeEquivalentTo(input);
    }

    [Fact]
    public void Can_Convert_Builder_To_Entity()
    {
        // Arrange
        var input = (IDialogPartBuilder)new MessageDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()));

        // Act
        var actual = input.Build();

        // Assert
        actual.Should().BeEquivalentTo(((MessageDialogPartBuilder)input).Build());
    }
}
