namespace DialogFramework.Domain.Tests.Builders;

public class DialogDefinitionBuilderTests
{
    private static DialogDefinitionBuilder CreateSut() => new DialogDefinitionBuilder();

    [Fact]
    public void Can_Validate_Recursively()
    {
        // Arrange
        var sut = CreateSut().AddSections(new DialogPartSectionBuilder().AddParts(new LabelDialogPartBuilder()));

        // Act
        var validationResults = new List<ValidationResult>();
        var success = sut.TryValidate(validationResults);

        // Assert
        success.ShouldBeFalse();
        validationResults.Select(x => x.ErrorMessage).ToArray().ShouldBeEquivalentTo
        (
            new[]
            {
                "The Id field is required.",
                "The Name field is required.",
                "The field Sections is invalid."
            }
        );
    }
}
