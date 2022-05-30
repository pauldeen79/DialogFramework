﻿namespace DialogFramework.Domain.DialogPartResultValues;

public record DateTimeDialogPartResultValue : IDialogPartResultValue
{
    public DateTimeDialogPartResultValue(DateTime value) => Value = value;
    public object? Value { get; }
    public ResultValueType ResultValueType => ResultValueType.DateTime;
}