﻿namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class SingleRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .WithValue(new DialogPartResultValueBuilder())
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("Answer is required");
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A"))
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialDefinitionog = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialDefinitionog.Metadata, sut);
        var results = new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("B")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        };

        // Act
        var result = sut.Validate(dialog, dialDefinitionog, results).ValidationErrors;

        // Assert
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
