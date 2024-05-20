namespace DialogFramework.Domain.DialogPartResults;

public partial class MultipleQuestionDialogPartResult<T>
{
    public override Result<object?> GetValue() => Result.Success<object?>(Values);
}
