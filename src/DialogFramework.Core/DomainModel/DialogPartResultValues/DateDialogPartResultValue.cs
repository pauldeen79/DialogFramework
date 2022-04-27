namespace DialogFramework.Core.DomainModel.DialogPartResultValues;

public record DateDialogPartResultValue : IDialogPartResultValue
{
    public DateDialogPartResultValue(DateTime value) => Value = value.Date;
    public object? Value { get; }
    public ResultValueType ResultValueType => ResultValueType.Date;
}
