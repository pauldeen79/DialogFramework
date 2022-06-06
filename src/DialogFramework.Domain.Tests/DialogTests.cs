namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentState_Is_Not_InProgess()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanAbort_Returns_True_When_CurrentState_Is_InProgess_And_CurrentPart_Is_Not_AbortedPart()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Abort_Throws_When_CanAbort_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Abort_Updates_State_Correcly_When_CanAbort_Is_True()
    {
        //verify CurrentPartId, CurrentGroupId and State
        throw new NotImplementedException();
    }

    [Fact]
    public void CanContinue_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanContinue_Returns_True_When_CurrentState_Is_InProgress()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Continue_Throws_When_CanContinue_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Continue_Updates_State_Correctly_When_CanContinue_Is_True()
    {
        //verify CurrentPartId (which is nextpartid), CurrentGroupId and CurrentState (based on next part), merged results and validation results (replaced)
        throw new NotImplementedException();
    }

    [Fact]
    public void Error_Updates_State_Correctly()
    {
        //verify CurrentPartId (which is Errorpart.Id), CurrentGroupId and CurrentState (based on Errorpart), Errors (overwritten with errors argument) and ValidationErrors (cleared)
        throw new NotImplementedException();
    }

    [Fact]
    public void CanStart_Returns_True_When_CurrentState_Is_Initial_And_Metadata_CanStart_Is_True()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanStart_Returns_False_When_CurrentState_Is_Not_Initial()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanStart_Returns_False_When_Metadata_CanStart_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanStart_Returns_False_When_Dialog_Does_Not_Have_A_First_Part()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Start_Throws_When_CanStart_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void Start_Updates_State_Correctly_When_CanStart_Is_True()
    {
        //verify CurrentPartId (which is firstpartid), CurrentGroupId and CurrentState (based on first part)
        throw new NotImplementedException();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Dialog_CanNavigateTo_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_CurrentState_Is_InProgress_And_Dialog_CanNavigateTo_Is_True()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void NavigateTo_Throws_When_CanNavigateTo_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void NavigateTo_Updates_State_Correctly_When_CanNavigateTo_Is_True()
    {
        //verify CurrentPartId (which is navigate to partid), CurrentGroupId and CurrentState (based on navigated part), and ValidationResults (cleared)
        throw new NotImplementedException();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_Dialog_CanResetResultsByPartId_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void CanResetCurrentState_Returns_True_When_CurrentState_Is_InProgress_And_Dialog_CanResetResultsByPartId_Is_True()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void ResetCurrentState_Throws_When_CanResetCurrentState_Is_False()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public void ResetCurrentState_Updates_Results_Correctly_When_CanResetCurrentState_Is_True()
    {
        // Arrange
        var dialog = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialog context = DialogFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context.Continue(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }, conditionEvaluatorMock.Object);
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().ContainSingle();
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 1 - Call reset while there is an answer
        context.ResetCurrentState(dialog);
        // Assert 1
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();

        // Act 2 - Call reset while there is no answer
        context.ResetCurrentState(dialog);
        // Assert 2
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();
    }

    [Fact]
    public void ResetCurrentState_Clears_ValidationErrors_When_CanResetCurrentState_Is_True()
    {
        throw new NotImplementedException();
    }
}
