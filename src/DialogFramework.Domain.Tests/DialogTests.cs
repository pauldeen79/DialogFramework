namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    [Fact]
    public void Constructing_Dialog_With_Null_Id_Throws()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(default(string)!);

        // Act
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }

    [Fact]
    public void Constructing_Dialog_With_Empty_Id_Throws()
    {
        // Arrange
        var builder = new DialogBuilder().WithId(string.Empty);

        // Act
        builder.Invoking(x => x.Build()).Should().Throw<ValidationException>();
    }
}
