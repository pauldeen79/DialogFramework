namespace DialogFramework.Domain.Tests.ValidationRules;

public class ConditionalRequiredValidationRuleBuilderTests
{
    [Fact]
    public void Ctor_Throws_On_Null_Condition()
    {
        // Arrange
        var builder = new ConditionalRequiredValidationRuleBuilder().WithCondition(default(EvaluatableBuilder)!);

        // Act
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }
}
