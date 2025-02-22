namespace DialogFramework.Domain.Tests.DialogParts;

public class SingleOpenQuestionDialogPartTests
{
    [Fact]
    public void Validate_Returns_Correct_ValidationResult()
    {
        // Arrange
        var sut = new SingleOpenQuestionDialogPartBuilder()
            .WithId("id")
            .WithTitle("title")
            .AddValidationRules(new RequiredValidationRuleBuilder())
            .BuildTyped();

        // Act
        var result = sut.Validate(default(int?), TestDialogFactory.CreateEmpty());

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
    }
}
