namespace DialogFramework.Domain.Tests;

public class DialogDefinitionTests
{
    private readonly Mock<IConditionEvaluator> _conditionEvaluatorMock;

    public DialogDefinitionTests()
    {
        _conditionEvaluatorMock = new Mock<IConditionEvaluator>();
    }

    [Fact]
    public void ReplaceAnswers_Replaces_Previous_Anwers_From_Same_Question()
    {
        // Arrange
        var dialogPartId1 = new DialogPartIdentifierBuilder().WithValue("1");
        var dialogPartId2 = new DialogPartIdentifierBuilder().WithValue("2");
        var result11 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId1).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("old")).Build();
        var result12 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId1).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("new")).Build();
        var result21 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId2).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("unchanged")).Build();
        var sut = DialogDefinitionFixture.CreateBuilder().Build();
        var oldResults = new[] { result11, result21 };
        var expectedResults = new[] { result21, result12 }; //note that as this time, the values are appended at the end. so order is not preserved (which is probably not a problem)

        // Act
        var actual = sut.ReplaceAnswers(oldResults, new[] { new DialogPartResultAnswer(result12.ResultId, result12.Value) }, dialogPartId1.Build());

        // Assert
        actual.Should().BeEquivalentTo(expectedResults, options => options.WithStrictOrdering());
    }

    [Fact]
    public void ResetPartResultsByPartId_Returns_Invalid_On_Non_QuestionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.ResetPartResultsByPartId(Enumerable.Empty<IDialogPartResult>(), sut.CompletedPart.Id);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void ResetPartResultsByPartId_Returns_NotFound_On_NonExisting_DialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.ResetPartResultsByPartId(Enumerable.Empty<IDialogPartResult>(), new DialogPartIdentifier("non existing id"));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = non existing id }]");
    }

    [Fact]
    public void ResetPartResultsByPartId_Returns_Correct_Result_When_CanResetPartResultsByPartId_Is_True()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();
        var otherResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Other"))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();
        var results = new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id)).WithResultId(new DialogPartResultIdentifierBuilder()).Build(),
            otherResult
        };

        // Act
        var actual = sut.ResetPartResultsByPartId(results, sut.Parts.First().Id);

        // Assert
        actual.IsSuccessful().Should().BeTrue();
        actual.Value.Should().BeEquivalentTo(new[] { otherResult });
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_DialogPart_Has_Not_Been_Processed_Yet()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.Parts.First().Id, sut.CompletedPart.Id, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.IsSuccessful().Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Has_Been_Processed()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.CompletedPart.Id, sut.Parts.First().Id, new[] { new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id)).WithResultId(new DialogPartResultIdentifierBuilder()).Build() } );

        // Assert
        actual.IsSuccessful().Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Is_CurrentPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.Parts.First().Id, sut.Parts.First().Id, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.IsSuccessful().Should().BeTrue();
    }

    [Fact]
    public void GetFirstPart_Returns_NotFound_When_Decision_Part_Returns_An_Unknown_PartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(new DecisionDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder())
                .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("non existing id")))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var result = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = non existing id }]");
    }

    [Fact]
    public void GetFirstPart_Returns_NotFound_When_Navigation_Part_Returns_An_Unknown_PartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(new NavigationDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder())
                .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("non existing id")))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var result = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = non existing id }]");
    }

    [Fact]
    public void GetFirstPart_Returns_CompletedPart_When_No_Other_Parts_Are_Available()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilderBase().Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var result = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        result.Value.Should().BeSameAs(sut.CompletedPart);
    }

    [Fact]
    public void GetFirstPart_Returns_First_Part_When_It_Is_A_Static_DialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.IsSuccessful().Should().BeTrue();
        actual.Value.Should().BeEquivalentTo(sut.Parts.First());
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Decision_From_First_Part_When_It_Is_A_DecisionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .With(x => x.Parts.Insert(0, new DecisionDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithDefaultNextPartId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.IsSuccessful().Should().BeTrue();
        actual.Value.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First());
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Navigation_From_First_Part_When_It_Is_A_NavigationDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .With(x => x.Parts.Insert(0, new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithNavigateToId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.IsSuccessful().Should().BeTrue();
        actual.Value.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First());
    }

    [Fact]
    public void GetNextPart_Returns_Invalid_With_ValidationErrors_When_Validation_Fails()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.OfType<IQuestionDialogPart>().First().Id);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult } );

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Validation failed, see ValidationErrors for more details");
        result.ValidationErrors.Should().ContainSingle();
    }

    [Fact]
    public void GetNextPart_Returns_NotFound_When_CurrentPartId_Is_Unknown()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata, new DialogPartIdentifier("non existing id"));
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult });

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = non existing id }]");
    }

    [Fact]
    public void GetNextPart_Returns_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_Static_DialogPart()
    {
        var sut = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult } );

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_DecisionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .With(x => x.Parts.Insert(1, new DecisionDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithDefaultNextPartId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult });

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_NavigationDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .With(x => x.Parts.Insert(1, new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithNavigateToId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult });

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_CompletedPart_When_Validation_Succeeds_And_There_Is_No_Next_Part()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .With(x => x.Parts.RemoveAt(1))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { partResult });

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.Id.Should().BeEquivalentTo(sut.CompletedPart.Id);
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void GetPartById_Returns_AbortedPart_When_Id_Is_AbortedPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.GetPartById(sut.AbortedPart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value.Should().BeSameAs(sut.AbortedPart);
    }

    [Fact]
    public void GetPartById_Returns_CompletedPart_When_Id_Is_CompletedPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.GetPartById(sut.CompletedPart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value.Should().BeSameAs(sut.CompletedPart);
    }

    [Fact]
    public void GetPartById_Returns_ErrorPart_When_Id_Is_ErrorPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.GetPartById(sut.ErrorPart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value.Should().BeSameAs(sut.ErrorPart);
    }

    [Fact]
    public void GetPartById_Returns_Part_From_Parts_Collection_When_Available_And_Id_Is_Not_Aborted_Completed_Or_Error()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.GetPartById(sut.Parts.First().Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value.Should().BeSameAs(sut.Parts.First());
    }

    [Fact]
    public void GetPartById_Returns_NotFound_When_Id_Is_Unknown_In_Dialog()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.GetPartById(new DialogPartIdentifier("non existing id"));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = non existing id }]");
    }

    [Fact]
    public void Constructing_DialogDefinition_With_Duplicate_Ids_Throws_ValidationException()
    {
        // Arrange
        var builder = DialogDefinitionFixture.CreateBuilder().With(x => x.AddParts(x.Parts.First()));

        // Act
        var act = new Action(() => _ = builder.Build());

        // Assert
        act.Should().ThrowExactly<ValidationException>();
    }
}
