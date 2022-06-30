namespace DialogFramework.Application.Tests.Requests;

public class AbortRequestTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(AbortRequest));
    }
}
