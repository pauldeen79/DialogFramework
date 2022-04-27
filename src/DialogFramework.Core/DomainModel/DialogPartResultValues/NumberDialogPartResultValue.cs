namespace DialogFramework.Core.DomainModel.DialogPartResultValues;

public record NumberDialogPartResultValue : IDialogPartResultValue
{
    public NumberDialogPartResultValue(decimal value) => Value = value;
    public object? Value { get; }
    public ResultValueType ResultValueType => ResultValueType.Number;
}
