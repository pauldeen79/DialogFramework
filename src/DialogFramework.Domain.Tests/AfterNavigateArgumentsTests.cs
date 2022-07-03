namespace DialogFramework.Domain.Tests;

public class AfterNavigateArgumentsTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(AfterNavigateArguments), parameterPredicate: pi => pi.Name != "currentGroupId" && pi.Name != "errorMessage");
    }

    [Fact]
    public void Can_Set_Result()
    {
        // Arrange
        var dialogMock = new Mock<IDialog>();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var sut = new AfterNavigateArguments(dialogMock.Object, conditionEvaluatorMock.Object, DialogAction.Continue);
        var result = Result.Error("Kaboom");

        // Act
        sut.SetResult(result);

        // Assert
        sut.Result.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Can_Add_Property()
    {
        // Arrange
        var dialogMock = new Mock<IDialog>();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var sut = new AfterNavigateArguments(dialogMock.Object, conditionEvaluatorMock.Object, DialogAction.Continue);
        var prop = new PropertyBuilder().WithName("MyName").WithValue("MyValue").Build();

        // Act
        sut.AddProperty(prop);

        // Assert
        dialogMock.Verify(x => x.AddProperty(prop), Times.Once);
    }
}
