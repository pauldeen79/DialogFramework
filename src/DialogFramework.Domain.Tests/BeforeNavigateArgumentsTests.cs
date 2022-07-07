namespace DialogFramework.Domain.Tests;

public class BeforeNavigateArgumentsTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(BeforeNavigateArguments), parameterPredicate: pi => pi.Name != "currentGroupId" && pi.Name != "errorMessage");
    }
}
