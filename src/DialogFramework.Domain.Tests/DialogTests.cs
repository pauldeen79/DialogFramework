using DialogFramework.Domain.Builders.DialogPartResults;

namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    [Fact]
    public void Constructing_Dialog_With_Null_Id_Throws()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(default(string)!);

        // Act & Assert
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }

    [Fact]
    public void Constructing_Dialog_With_Empty_Id_Throws()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(string.Empty);

        // Act & Assert
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }

    [Fact]
    public void Can_Validate_Builder_Before_Building_Entity()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(string.Empty);

        // Act
        var validationResults = builder.Validate(new ValidationContext(builder));

        // Assert
        validationResults.Select(x => x.ErrorMessage).Should().BeEquivalentTo("The Id field is required.", "The DefinitionId field is required.", "The DefinitionVersion field is required.");
    }

    [Fact]
    public void Can_Keep_State_Of_Dialog()
    {
        // Act
        var sut = new DialogBuilder()
            .WithId(Guid.NewGuid().ToString())
            .WithDefinitionId("MyDialog")
            .WithDefinitionVersion("1.0.0")
            .AddResults(new SingleQuestionDialogPartResultBuilder<string>().WithPartId("MyPart").WithValue("Paul Deen"))
            .Build();

        // Assert
        sut.Should().NotBeNull();
    }
}
