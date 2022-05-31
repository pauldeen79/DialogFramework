namespace DialogFramework.Domain.Tests.DialogParts;

public class ErrorDialogPartTests
{
    [Fact]
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new ErrorDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
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
        var input = (IDialogPartBuilder)new ErrorDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"));

        // Act
        var actual = input.Build();

        // Assert
        actual.Should().BeEquivalentTo(((ErrorDialogPartBuilder)input).Build());
    }
}
