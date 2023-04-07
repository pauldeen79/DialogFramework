namespace DialogFramework.Domain.DialogPartResults;

public partial record SingleQuestionDialogPartResult<T>
{
    public override object? GetValue() => Value;
}

