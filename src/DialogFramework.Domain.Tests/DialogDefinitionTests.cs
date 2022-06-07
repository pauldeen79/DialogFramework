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
        var actual = sut.ReplaceAnswers(oldResults, new[] { result12 });

        // Assert
        actual.Should().BeEquivalentTo(expectedResults, options => options.WithStrictOrdering());
    }

    [Fact]
    public void CanResetPartResultsByPartId_Returns_True_On_QuestionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanResetPartResultsByPartId(sut.Parts.OfType<IQuestionDialogPart>().First().Id);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanResetPartResultsByPartId_Returns_False_On_Non_QuestionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanResetPartResultsByPartId(sut.CompletedPart.Id);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void ResetPartResultsByPartId_Throws_When_CanResetPartResultsByPartId_Is_False()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var act = new Action(() => sut.ResetPartResultsByPartId(Enumerable.Empty<IDialogPartResult>(), sut.CompletedPart.Id));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
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
        actual.Should().BeEquivalentTo(new[] { otherResult });
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_DialogPart_Has_Not_Been_Processed_Yet()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.Parts.First().Id, sut.CompletedPart.Id, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Has_Been_Processed()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.CompletedPart.Id, sut.Parts.First().Id, new[] { new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id)).WithResultId(new DialogPartResultIdentifierBuilder()).Build() } );

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Is_CurrentPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanNavigateTo(sut.Parts.First().Id, sut.Parts.First().Id, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanStart_Returns_False_When_Dynamic_Part_Returns_An_Unknown_PartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(new DecisionDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder())
                .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Unknown")))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.CanStart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void GetFirstPart_Returns_CompletedPart_When_No_Other_Parts_Are_Available()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilderBase().Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeSameAs(sut.CompletedPart);
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
        actual.Should().BeEquivalentTo(sut.Parts.First());
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Decision_From_First_Part_When_It_Is_A_DecisionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .Chain(x => x.Parts.Insert(0, new DecisionDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithDefaultNextPartId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First());
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Navigation_From_First_Part_When_It_Is_A_NavigationDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .Chain(x => x.Parts.Insert(0, new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithNavigateToId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata);

        // Act
        var actual = sut.GetFirstPart(dialog, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First());
    }

    [Fact]
    public void GetNextPart_Returns_Current_Part_With_ValidationErrors_When_Validation_Fails()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.OfType<IQuestionDialogPart>().First().Id);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.OfType<IQuestionDialogPart>().First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { result } );

        // Assert
        actual.Id.Should().BeEquivalentTo(sut.Parts.OfType<IQuestionDialogPart>().First().Id); //first part
        actual.GetValidationResults().Should().NotBeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_Static_DialogPart()
    {
        var sut = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { result } );

        // Assert
        actual.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        actual.GetValidationResults().Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_DecisionDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .Chain(x => x.Parts.Insert(1, new DecisionDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithDefaultNextPartId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { result });

        // Assert
        actual.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        actual.GetValidationResults().Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_NavigationDialogPart()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .Chain(x => x.Parts.Insert(1, new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithNavigateToId(x.Parts.OfType<MessageDialogPartBuilder>().First().Id)))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { result });

        // Assert
        actual.Id.Should().BeEquivalentTo(sut.Parts.OfType<IMessageDialogPart>().First().Id); //second part
        actual.GetValidationResults().Should().BeEmpty();
    }

    [Fact]
    public void GetNextPart_Returns_CompletedPart_When_Validation_Succeeds_And_There_Is_No_Next_Part()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder()
            .Chain(x => x.Parts.RemoveAt(1))
            .Build();
        var dialog = DialogFixture.Create(sut.Metadata, sut.Parts.First().Id);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.GetNextPart(dialog, _conditionEvaluatorMock.Object, new[] { result });

        // Assert
        actual.Id.Should().BeEquivalentTo(sut.CompletedPart.Id);
        actual.GetValidationResults().Should().BeEmpty();
    }

    [Fact]
    public void GetPartById_Returns_AbortedPart_When_Id_Is_AbortedPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.GetPartById(sut.AbortedPart.Id);

        // Assert
        actual.Should().BeSameAs(sut.AbortedPart);
    }

    [Fact]
    public void GetPartById_Returns_CompletedPart_When_Id_Is_CompletedPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.GetPartById(sut.CompletedPart.Id);

        // Assert
        actual.Should().BeSameAs(sut.CompletedPart);
    }

    [Fact]
    public void GetPartById_Returns_ErrorPart_When_Id_Is_ErrorPartId()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.GetPartById(sut.ErrorPart.Id);

        // Assert
        actual.Should().BeSameAs(sut.ErrorPart);
    }

    [Fact]
    public void GetPartById_Returns_Part_From_Parts_Collection_When_Available_And_Id_Is_Not_Aborted_Completed_Or_Error()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var actual = sut.GetPartById(sut.Parts.First().Id);

        // Assert
        actual.Should().BeSameAs(sut.Parts.First());
    }

    [Fact]
    public void GetPartById_Throws_When_Id_Is_Unknown_In_Dialog()
    {
        // Arrange
        var sut = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var act = new Action(() => _ = sut.GetPartById(new DialogPartIdentifier("unknown")));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Constructing_DialogDefinition_With_Duplicate_Ids_Throws_ValidationException()
    {
        // Arrange
        var builder = DialogDefinitionFixture.CreateBuilder().Chain(x => x.AddParts(x.Parts.First()));

        // Act
        var act = new Action(() => _ = builder.Build());

        // Assert
        act.Should().ThrowExactly<ValidationException>();
    }
}
