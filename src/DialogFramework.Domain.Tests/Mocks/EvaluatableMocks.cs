namespace DialogFramework.Domain.Tests.Mocks;

[ExcludeFromCodeCoverage]
internal sealed record MyEvaluatableWithEmptyErrorMessage : Evaluatable
{
    public override Result<bool> Evaluate(object? context) => Result.Error<bool>(); // no error message on purpose
}
[ExcludeFromCodeCoverage]
internal sealed class MyEvaluatableWithEmptyErrorMessageBuilder : EvaluatableBuilder
{
    public override Evaluatable Build() => new MyEvaluatableWithEmptyErrorMessage();
}
