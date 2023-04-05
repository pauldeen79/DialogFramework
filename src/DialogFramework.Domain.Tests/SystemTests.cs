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
        result.Should().BeTrue();
        validationResult.Should().BeEmpty();
    }
}
