namespace DialogFramework.Domain.DialogPartResults;

public partial record MultipleQuestionDialogPartResult<T>
{
    public override Result<object?> GetValue() => Result<object?>.Success(Values);
}

