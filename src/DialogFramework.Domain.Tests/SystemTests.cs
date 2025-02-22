namespace DialogFramework.Domain.Tests;

public class SystemTests
{
    [Required]
    public int Value { get; set; }

    /// <summary>
    /// This test proves that the built-in Required attribute from Microsoft accepts 0 as a valid value for an integer. It only has a special case for strings (string.IsNullOrEmpty)
    /// </summary>
    [Fact]
    public void Int32_Zero_Is_Accepted_As_Valid_On_RequiredAttribute()
    {
        // Arrange
        Value = 0;
        var validationResult = new List<ValidationResult>();

        // Act
        var result = this.TryValidate(validationResult);

        // Assert
        result.ShouldBeTrue();
        validationResult.ShouldBeEmpty();
    }

    [Fact]
    public void Can_Use_Null_In_Compare_As_Source()
    {
        // Arrange
        var sut = new DialogBuilder().WithId("id").WithDefinitionId("definitionId").Build();

        // Act
        ///sut.Context = new object(); // can't do this because the setter is private
        sut.GetType().GetProperty(nameof(sut.Context))!.SetValue(sut, new object());

        // Assert
        sut.Context.ShouldNotBeNull();
    }

    [Fact]
    public void Can_Use_Null_In_Compare_As_Target()
    {
        // Arrange
        var sut = new DialogBuilder().WithId("id").WithDefinitionId("definitionId").WithContext(new object()).Build();

        // Act
        ///sut.Context = null; // can't do this because the setter is private
        sut.GetType().GetProperty(nameof(sut.Context))!.SetValue(sut, null);

        // Assert
        sut.Context.ShouldBeNull();
    }
}
