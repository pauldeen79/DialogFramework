namespace DialogFramework.Domain.Tests;

public class AfterNavigateArgumentsTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(AfterNavigateArguments), parameterPredicate: pi => pi.Name != "currentGroupId" && pi.Name != "errorMessage");
    }

    [Fact]
    public void Can_Add_Property()
    {
        // Arrange
        var dialogMock = new Mock<IDialog>();
        var definitionMock = new Mock<IDialogDefinition>();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var sut = new AfterNavigateArguments(dialogMock.Object, definitionMock.Object, conditionEvaluatorMock.Object, DialogAction.Continue);
        var prop = new PropertyBuilder().WithName("MyName").WithValue("MyValue").Build();

        // Act
        sut.AddProperty(prop);

        // Assert
        dialogMock.Verify(x => x.AddProperty(prop), Times.Once);
    }

    [Fact]
    public void Constructor_Copies_State_Correctly_From_Input_Dialog()
    {
        // Arrange
        var definition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", definition.Metadata, definition.Parts.First());
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();

        // Act
        var sut = new AfterNavigateArguments(dialog, definition, conditionEvaluatorMock.Object, DialogAction.NavigateTo);

        // Assert
        sut.Action.Should().Be(DialogAction.NavigateTo);
        sut.Evaluator.Should().BeSameAs(conditionEvaluatorMock.Object);
        sut.CurrentDialogId.Should().BeEquivalentTo(new DialogIdentifier("Id"));
        sut.DefinitionId.Should().BeEquivalentTo(new DialogDefinitionIdentifierBuilder(definition.Metadata).Build());
        sut.CurrentGroupId.Should().BeEquivalentTo(dialog.CurrentGroupId);
        sut.CurrentPartId.Should().BeEquivalentTo(dialog.CurrentPartId);
        sut.CurrentState.Should().Be(dialog.CurrentState);
        sut.Definition.Should().BeSameAs(definition);
        sut.ErrorMessage.Should().Be(dialog.ErrorMessage);
    }
}
