namespace DialogFramework.Domain.Tests;

public class DialogPartTests
{
    [Fact]
    public void Validate_Returns_Result_When_Error()
    {
        // Arrange
        var sut = new SingleOpenQuestionDialogPartBuilder().WithId("Id").WithTitle("Title").AddValidationRules(new MaliciousValidationRuleBuilder()).BuildTyped();

        // Act
        var result = sut.Validate("some value", TestDialogFactory.CreateEmpty(), sut.ValidationRules);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.ErrorMessage.ShouldBe("Kaboom");
    }

    private sealed class MaliciousValidationRuleBuilder : ValidationRuleBuilder
    {
        public override ValidationRule Build() => new MaliciousValidationRule();
    }

    private sealed class MaliciousValidationRule : ValidationRule
    {
        public override ValidationRuleBuilder ToBuilder()
        {
            throw new NotImplementedException();
        }

        public override Result Validate<T>(string id, T value, Dialog dialog) => Result.Error("Kaboom");
    }
}
