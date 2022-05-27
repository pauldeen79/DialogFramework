namespace DialogFramework.Domain.Tests.DialogParts;

public class QuestionDialogPartTests
{
    [Fact]
    public void Validate_With_Unknown_Id_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var results = new[] { new DialogPartResult(sut.Id, "C", new YesNoDialogPartResultValue(true)) };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Unknown Result Id: [C]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Wrong_ValueType_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var results = new[] { new DialogPartResult(sut.Id, "A", new EmptyDialogPartResultValue()) };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Result for [Test.A] should be of type [YesNo], but type [None] was answered");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var sut = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var results = new[] { new DialogPartResult(sut.Id, "A", new YesNoDialogPartResultValue(true)) };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, context, dialog, results).ValidationErrors;

        // Assert
        actual.Should().BeEmpty();
    }
}
