﻿namespace DialogFramework.Domain.Tests;

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

    [Fact]
    public void Ctor_Throws_On_Duplicate_SectionIds()
    {
        // Arrange
        var builder = new DialogDefinitionBuilder()
            .WithId("Test")
            .WithVersion("1.0.0")
            .WithName("Test dialog definition")
            .AddSections(
                new DialogPartSectionBuilder().WithId("Id1").WithName("Name1"),
                new DialogPartSectionBuilder().WithId("Id1").WithName("Name2")
            );

        // Act
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>().WithMessage("Duplicate section ids: Id1");
    }

    [Fact]
    public void Ctor_Throws_On_Duplicate_PartIds()
    {
        // Arrange
        var builder = new DialogDefinitionBuilder()
            .WithId("Test")
            .WithVersion("1.0.0")
            .WithName("Test dialog definition")
            .AddSections(
                new DialogPartSectionBuilder()
                    .WithId("Id")
                    .WithName("Name")
                    .AddParts(
                        new LabelDialogPartBuilder().WithId("Id1").WithTitle("Title1"),
                        new LabelDialogPartBuilder().WithId("Id1").WithTitle("Title2")
                    )
            );

        // Act
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>().WithMessage("Duplicate part ids: Id1");
    }
}
