namespace DialogFramework.Domain.DialogPartResults;

public partial record MultipleQuestionDialogPartResult<T>
{
    public override object? GetValue() => Values;
}

