namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class SingleRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(sut.Id)
            .WithValue(new DialogPartResultValueBuilder())
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, conditionEvaluator, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Answer is required");
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(sut.Id)
            .WithResultId("A")
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, conditionEvaluator, results).ValidationErrors;

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var results = new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("A").WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("B").WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, conditionEvaluator, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
