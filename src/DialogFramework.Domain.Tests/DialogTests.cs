namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    [Fact]
    public void ReplaceAnswers_Replaces_Previous_Anwers_From_Same_Question()
    {
        // Arrange
        var dialogPartId1 = new DialogPartIdentifierBuilder().WithValue("1");
        var dialogPartId2 = new DialogPartIdentifierBuilder().WithValue("2");
        var result11 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId1).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("old")).Build();
        var result12 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId1).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("new")).Build();
        var result21 = new DialogPartResultBuilder().WithDialogPartId(dialogPartId2).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("unchanged")).Build();
        var sut = DialogFixture.CreateBuilder().Build();
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
        var sut = DialogFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanResetPartResultsByPartId(sut.Parts.OfType<IQuestionDialogPart>().First().Id);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanResetPartResultsByPartId_Returns_False_On_Non_QuestionDialogPart()
    {
        // Arrange
        var sut = DialogFixture.CreateBuilder().Build();

        // Act
        var actual = sut.CanResetPartResultsByPartId(sut.CompletedPart.Id);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void ResetPartResultsByPartId_Throws_When_CanResetPartResultsByPartId_Is_False()
    {
        // Arrange
        var sut = DialogFixture.CreateBuilder().Build();

        // Act
        var act = new Action(() => sut.ResetPartResultsByPartId(Enumerable.Empty<IDialogPartResult>(), sut.CompletedPart.Id));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void ResetPartResultsByPartId_Returns_Correct_Result_When_CanResetPartResultsByPartId_Is_True()
    {
        // Arrange
        var sut = DialogFixture.CreateBuilder().Build();
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
        throw new NotImplementedException();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Has_Been_Processed()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_DialogPart_Is_CurrentPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetFirstPart_Throws_Wnen_No_Parts_Are_Available() //TODO: move to validation in c'tor
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetFirstPart_Returns_First_Part_When_It_Is_A_Static_DialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Decision_From_First_Part_When_It_Is_A_DecisionDialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetFirstPart_Returns_Processed_Navigation_From_First_Part_When_It_Is_A_NavigationDialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetNextPart_Returns_Current_Part_With_ValidationErrors_When_Validation_Fails()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetNextPart_Returns_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_Static_DialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_DecisionDialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetNextPart_Returns_Processed_Decision_From_Next_Part_When_Validation_Succeeds_And_Next_Part_Is_A_NavigationDialogPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetNextPart_Returns_CompletedPart_When_Validation_Succeeds_And_There_Is_No_Next_Part()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Returns_AbortedPart_When_Id_Is_AbortedPartId()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Returns_CompletedPart_When_Id_Is_CompletedPartId()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Returns_ErrorPart_When_Id_Is_ErrorPartId()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Returns_Part_From_Parts_Collection_When_Available_And_Id_Is_Not_Aborted_Completed_Or_Error()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Throws_When_Id_Is_Unknown_In_Dialog()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void GetPartById_Throws_When_Id_Occurs_Multiple_Times_In_Dialog() //TODO: Add validation that all Ids need to be unique (including error, aborted and completed)
    {
        throw new NotImplementedException();
    }
}
