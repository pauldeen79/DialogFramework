namespace DialogFramework.Application.Tests.Requests;

public class ResetStateRequestTests
{
    [Fact]
    public void Construction_Should_Throw_On_Null_Arguments()
    {
        TestHelpers.ConstructorMustThrowArgumentNullException(typeof(ResetStateRequest));
    }
}
