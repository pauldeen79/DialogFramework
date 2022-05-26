namespace DialogFramework.Domain.DomainModel.DialogPartResultValues;

public record YesNoDialogPartResultValue : IDialogPartResultValue
{
    public YesNoDialogPartResultValue(bool value) => Value = value;
    public object? Value { get; }
    public ResultValueType ResultValueType => ResultValueType.YesNo;
}
