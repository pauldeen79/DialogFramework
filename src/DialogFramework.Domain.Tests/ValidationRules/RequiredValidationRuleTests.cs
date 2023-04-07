namespace DialogFramework.Domain.Tests.ValidationRules;

public class RequiredValidationRuleTests
{
    [Fact]
    public void Validate_Returns_Success_When_Value_Is_Not_Null()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.Create();

        // Act
        var actual = sut.Validate("MyId", "filled", dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Value_Is_Null()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.Create();

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Invalid);
        actual.ValidationErrors.Should().ContainSingle();
        actual.ValidationErrors.First().ErrorMessage.Should().Be("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.Should().BeEquivalentTo("MyId");
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Value_Is_Empty_String()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.Create();

        // Act
        var actual = sut.Validate("MyId", string.Empty, dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Invalid);
        actual.ValidationErrors.Should().ContainSingle();
        actual.ValidationErrors.First().ErrorMessage.Should().Be("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.Should().BeEquivalentTo("MyId");
    }

    [Fact]
    public void Validate_Returns_Success_When_Value_Is_Default_Int32()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.Create();

        // Act
        var actual = sut.Validate("MyId", default(int), dialog);

        // Assert
        actual.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void BaseClass_Cannot_Validate()
    {
        // Arrange
        var sut = new RequiredValidationRuleBase();
        var dialog = new DialogBuilder().WithDefinitionId("MyDialogDefinition").WithDefinitionVersion("1.0.0").WithId("Wrong").Build();

        // Act
        var result = sut.Validate("Id", false, dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.NotSupported);
    }
}
