namespace DialogFramework.Domain.Tests.DialogParts;

public class NavigationDialogPartTests
{
    [Fact]
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new NavigationDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithNavigateToId(new DialogPartIdentifierBuilder())
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
        var input = (IDialogPartBuilder)new NavigationDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithNavigateToId(new DialogPartIdentifierBuilder());

        // Act
        var actual = input.Build();

        // Assert
        actual.Should().BeEquivalentTo(((NavigationDialogPartBuilder)input).Build());
    }
}
