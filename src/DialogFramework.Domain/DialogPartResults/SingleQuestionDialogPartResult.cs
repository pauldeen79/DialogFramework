namespace DialogFramework.Domain.DialogPartResults;

public partial class SingleQuestionDialogPartResult<T>
{
    public override Result<object?> GetValue() => Result.Success<object?>(Value);
}
