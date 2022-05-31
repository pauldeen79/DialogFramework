namespace DialogFramework.Domain.Tests.DialogParts;

public class RedirectDialogPartTests
{
    [Fact]
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new RedirectDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithRedirectDialogMetadata(new DialogMetadataBuilder())
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
        var input = (IDialogPartBuilder)new RedirectDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithRedirectDialogMetadata(new DialogMetadataBuilder());

        // Act
        var actual = input.Build();

        // Assert
        actual.Should().BeEquivalentTo(((RedirectDialogPartBuilder)input).Build());
    }
}
