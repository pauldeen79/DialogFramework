namespace DialogFramework.Domain.Tests.DialogParts;

public class MultipleOpenQuestionDialogPartTests
{
    [Fact]
    public void Validate_Returns_Correct_ValidationResult()
    {
        // Arrange
        var sut = new MultipleOpenQuestionDialogPartBuilder()
            .WithId("id")
            .WithTitle("title")
            .AddValidationRules(new RequiredValidationRuleBuilder())
            .BuildTyped();

        // Act
        var result = sut.Validate(Array.Empty<string>(), TestDialogFactory.CreateEmpty());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }
}
