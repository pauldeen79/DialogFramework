namespace DialogFramework.Domain.Tests.ValidationRules;

public class ConditionalRequiredValidationRuleBuilderTests
{
    [Fact]
    public void Ctor_Throws_On_Null_Condition()
    {
        // Arrange
        var builder = new ConditionalRequiredValidationRuleBuilder().WithCondition(default(EvaluatableBuilder)!);

        // Act & Assert
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_Returns_Success_When_Condition_Is_False_And_Value_Is_Null()
    {
        // Arrange
        var sut = new ConditionalRequiredValidationRuleBuilder()
            .WithCondition(
                new SingleEvaluatableBuilder()
                    .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new TypedConstantExpressionBuilder<string>().WithValue(nameof(Dialog.Id))).WithExpression(new ContextExpressionBuilder()))
                    .WithOperator(new EqualsOperatorBuilder())
                    .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
            )
            .BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty(id: "Wrong");

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void Validate_Returns_Success_When_Condition_Is_True_But_Value_Is_Not_Null()
    {
        // Arrange
        var sut = new ConditionalRequiredValidationRuleBuilder()
            .WithCondition(
                new SingleEvaluatableBuilder()
                    .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new TypedConstantExpressionBuilder<string>().WithValue(nameof(Dialog.Id))).WithExpression(new ContextExpressionBuilder()))
                    .WithOperator(new EqualsOperatorBuilder())
                    .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
            )
            .BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", "filled", dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Condition_Is_True_And_Value_Is_Null()
    {
        // Arrange
        var sut = new ConditionalRequiredValidationRuleBuilder()
            .WithCondition(
                new SingleEvaluatableBuilder()
                    .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new TypedConstantExpressionBuilder<string>().WithValue(nameof(Dialog.Id))).WithExpression(new ContextExpressionBuilder()))
                    .WithOperator(new EqualsOperatorBuilder())
                    .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
            )
            .BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Invalid);
        actual.ValidationErrors.Should().ContainSingle();
        actual.ValidationErrors.First().ErrorMessage.Should().Be("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.Should().BeEquivalentTo("MyId");
    }

    [Fact]
    public void Validate_Returns_Error_When_ConditionEvaluation_Fails()
    {
        // Arrange
        var sut = new ConditionalRequiredValidationRuleBuilder()
            .WithCondition(
                new SingleEvaluatableBuilder()
                    .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new TypedConstantExpressionBuilder<string>().WithValue("NonExistingPropertyName")).WithExpression(new ContextExpressionBuilder()))
                    .WithOperator(new EqualsOperatorBuilder())
                    .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
            )
            .BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Error);
        actual.ErrorMessage.Should().Be("Fieldname [NonExistingPropertyName] is not found on type [DialogFramework.Domain.Dialog]");
    }

    [Fact]
    public void Validate_Returns_Error_With_NonEmpty_ErrorMessage_When_ConditionEvaluation_Fails_Without_ErrorMessage()
    {
        // Arrange
        var sut = new ConditionalRequiredValidationRuleBuilder()
            .WithCondition(new MyEvaluatableWithEmptyErrorMessageBuilder())
            .BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Error);
        actual.ErrorMessage.Should().Be("Condition evaluation failed");
    }
}
