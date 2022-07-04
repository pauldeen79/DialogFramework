namespace DialogFramework.Domain.Tests;

public class BeforeNavigateArgumentsTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(BeforeNavigateArguments), parameterPredicate: pi => pi.Name != "currentGroupId" && pi.Name != "errorMessage");
    }

    [Fact]
    public void Can_Cancel_StateUpdate()
    {
        // Arrange
        var dialogMock = new Mock<IDialog>();
        var definitionMock = new Mock<IDialogDefinition>();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var sut = new BeforeNavigateArguments(dialogMock.Object, definitionMock.Object, conditionEvaluatorMock.Object, DialogAction.Continue);

        // Act
        sut.CancelStateUpdate();

        // Assert
        sut.UpdateState.Should().BeFalse();
    }
}
