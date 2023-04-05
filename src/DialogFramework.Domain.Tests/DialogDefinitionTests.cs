namespace DialogFramework.Domain.Tests;

public class DialogDefinitionTests
{
    [Theory,
        InlineData("Wrong", false),
        InlineData("Correct", true)]
    public void Can_Conditionally_Show_Section(string propertyValue, bool conditionResult)
    {
        // Arrange
        var sut = new DialogDefinitionBuilder()
            .WithId("MyNamespace.MyId")
            .WithName("MyDialog")
            .WithVersion("1.0.0")
            .AddSections(
                new DialogPartSectionBuilder()
                    .WithId("PersonalDetails")
                    .WithName("Personal details")
                    .WithCondition(
                        new SingleEvaluatableBuilder()
                            .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new ConstantExpressionBuilder().WithValue("PropertyName")).WithExpression(new ContextExpressionBuilder()))
                            .WithOperator(new EqualsOperatorBuilder())
                            .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
                    )
                    .AddParts(
                        new LabelDialogPartBuilder()
                            .WithId("Welcome")
                            .WithTitle("Welcome to this great app!")
                    )
            ).Build();

        // Act
        sut.Sections.Should().ContainSingle();
        sut.Sections.First().Condition.Should().NotBeNull();
        sut.Sections.First().Condition!.Evaluate(new { PropertyName = propertyValue }).Value.Should().Be(conditionResult);
    }

    [Theory,
        InlineData("Wrong", false),
        InlineData("Correct", true)]
    public void Can_Conditionally_Show_Question(string propertyValue, bool conditionResult)
    {
        // Arrange
        var sut = new DialogDefinitionBuilder()
            .WithId("MyNamespace.MyId")
            .WithName("MyDialog")
            .WithVersion("1.0.0")
            .AddSections(
                new DialogPartSectionBuilder()
                    .WithId("PersonalDetails")
                    .WithName("Personal details")
                    .AddParts(
                        new LabelDialogPartBuilder()
                            .WithId("Welcome")
                            .WithTitle("Welcome to this great app!")
                            .WithCondition(
                                new SingleEvaluatableBuilder()
                                    .WithLeftExpression(new FieldExpressionBuilder().WithFieldNameExpression(new ConstantExpressionBuilder().WithValue("PropertyName")).WithExpression(new ContextExpressionBuilder()))
                                    .WithOperator(new EqualsOperatorBuilder())
                                    .WithRightExpression(new ConstantExpressionBuilder().WithValue("Correct"))
                            )
                    )
            ).Build();

        // Act
        sut.Sections.Should().ContainSingle();
        sut.Sections.First().Parts.Should().ContainSingle();
        sut.Sections.First().Parts.First().Condition.Should().NotBeNull();
        sut.Sections.First().Parts.First().Condition!.Evaluate(new { PropertyName = propertyValue }).Value.Should().Be(conditionResult);
    }
}
