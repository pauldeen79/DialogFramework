namespace DialogFramework.Domain.Tests.DialogParts;

public class MultipleClosedQuestionDialogPartTests
{
    [Fact]
    public void Validate_Returns_Correct_ValidationResult()
    {
        // Arrange
        var sut = new MultipleClosedQuestionDialogPartBuilder()
            .WithId("id")
            .WithTitle("title")
            .AddValidationRules(new RequiredValidationRuleBuilder())
            .BuildTyped();

        // Act
        var result = sut.Validate(Array.Empty<int>(), TestDialogFactory.CreateEmpty());

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
    }
}
