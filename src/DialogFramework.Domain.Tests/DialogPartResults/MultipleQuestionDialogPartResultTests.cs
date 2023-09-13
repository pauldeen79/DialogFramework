namespace DialogFramework.Domain.Tests.DialogPartResults;

public class MultipleQuestionDialogPartResultTests
{
    [Fact]
    public void GetValue_Returns_Value()
    {
        // Arrange
        var sut = new MultipleQuestionDialogPartResultBuilder<string>().WithPartId("Test").AddValues("test").Build();

        // Act
        var result = sut.GetValue();

        // Assert
        result.GetValueOrThrow().Should().BeEquivalentTo(new[] { "test" });
    }
}
