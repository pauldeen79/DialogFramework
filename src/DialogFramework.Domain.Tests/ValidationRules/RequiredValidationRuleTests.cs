namespace DialogFramework.Domain.Tests.ValidationRules;

public class RequiredValidationRuleTests
{
    [Fact]
    public void Validate_Returns_Success_When_Value_Is_Not_Null()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", "filled", dialog);

        // Assert
        actual.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Value_Is_Null()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", default(string?), dialog);

        // Assert
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.ValidationErrors.ShouldHaveSingleItem();
        actual.ValidationErrors.First().ErrorMessage.ShouldBe("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.ToArray().ShouldBeEquivalentTo(new[] { "MyId" });
    }

    [Fact]
    public void Validate_Returns_Invalid_When_Value_Is_Empty_String()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", string.Empty, dialog);

        // Assert
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.ValidationErrors.ShouldHaveSingleItem();
        actual.ValidationErrors.First().ErrorMessage.ShouldBe("The MyId field is required.");
        actual.ValidationErrors.First().MemberNames.ToArray().ShouldBeEquivalentTo(new[] { "MyId" });
    }

    [Fact]
    public void Validate_Returns_Success_When_Value_Is_Default_Int32()
    {
        // Arrange
        var sut = new RequiredValidationRuleBuilder().BuildTyped();
        var dialog = TestDialogFactory.CreateEmpty();

        // Act
        var actual = sut.Validate("MyId", default(int), dialog);

        // Assert
        actual.Status.ShouldBe(ResultStatus.Ok);
    }
}
