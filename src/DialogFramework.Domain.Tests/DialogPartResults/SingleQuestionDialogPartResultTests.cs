namespace DialogFramework.Domain.Tests.DialogPartResults;

public class SingleQuestionDialogPartResultTests
{
    [Fact]
    public void GetValue_Returns_Value()
    {
        // Arrange
        var sut = new SingleQuestionDialogPartResultBuilder<string>().WithPartId("Test").WithValue("test").Build();

        // Act
        var result = sut.GetValue();

        // Assert
        result.GetValueOrThrow().ShouldBeEquivalentTo("test");
    }
}
