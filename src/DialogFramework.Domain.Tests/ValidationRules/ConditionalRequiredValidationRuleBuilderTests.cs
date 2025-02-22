namespace DialogFramework.Domain.Tests.ValidationRules;

public class ConditionalRequiredValidationRuleBuilderTests
{
    [Fact]
    public void WithCondition_Throws_On_Null_Condition()
    {
        // Arrange
        var builder = new ConditionalRequiredValidationRuleBuilder();

        // Act & Assert
        Action a = () => builder.WithCondition(default!);
        a.ShouldThrow<ArgumentNullException>();
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
        actual.Status.ShouldBe(ResultStatus.Ok);
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
        actual.Status.ShouldBe(ResultStatus.Ok);
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
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.ValidationErrors.ShouldHaveSingleItem();
        actual.ValidationErrors.First().ErrorMessage.ShouldBe("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.ToArray().ShouldBeEquivalentTo(new[] { "MyId" });
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
        actual.Status.ShouldBe(ResultStatus.Error);
        actual.ErrorMessage.ShouldBe("Fieldname [NonExistingPropertyName] is not found on type [DialogFramework.Domain.Dialog]");
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
        actual.Status.ShouldBe(ResultStatus.Error);
        actual.ErrorMessage.ShouldBe("Condition evaluation failed");
    }
}
