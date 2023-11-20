namespace DialogFramework.Domain.DialogPartResults;

public partial record MultipleQuestionDialogPartResult<T>
{
    public override Result<object?> GetValue() => Result.Success<object?>(Values);
}

