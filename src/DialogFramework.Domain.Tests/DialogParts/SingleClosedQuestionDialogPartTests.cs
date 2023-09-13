namespace DialogFramework.Domain.Tests.DialogParts;

public class SingleClosedQuestionDialogPartTests
{
    [Fact]
    public void Validate_Returns_Correct_ValidationResult()
    {
        // Arrange
        var sut = new SingleClosedQuestionDialogPartBuilder()
            .WithId("id")
            .WithTitle("title")
            .AddValidationRules(new RequiredValidationRuleBuilder())
            .BuildTyped();

        // Act
        var result = sut.Validate(string.Empty, TestDialogFactory.CreateEmpty());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }
}
