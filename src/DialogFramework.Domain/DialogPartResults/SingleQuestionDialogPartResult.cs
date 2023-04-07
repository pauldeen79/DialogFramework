namespace DialogFramework.Domain.DialogPartResults;

public partial record SingleQuestionDialogPartResult<T>
{
    public override Result<object?> GetValue() => Result<object?>.Success(Value);
}

