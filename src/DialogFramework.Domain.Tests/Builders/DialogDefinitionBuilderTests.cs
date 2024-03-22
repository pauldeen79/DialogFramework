namespace DialogFramework.Domain.Tests.Builders;

public class DialogDefinitionBuilderTests
{
    private static DialogDefinitionBuilder CreateSut() => new DialogDefinitionBuilder();

    [Fact(Skip = "Can only pass after fixing problem in ClassFramework, and then replace ModelFramework code generation in DialogFramework with ClassFramework")]
    public void Can_Validate_Recursively()
    {
        // Arrange
        var sut = CreateSut().AddSections(new DialogPartSectionBuilder().AddParts(new LabelDialogPartBuilder()));

        // Act
        var validationResults = new List<ValidationResult>();
        var success = sut.TryValidate(validationResults);

        // Assert
        success.Should().BeFalse();
        validationResults.Should().HaveCount(2); //both the validation errors in Class and Field
    }
}
