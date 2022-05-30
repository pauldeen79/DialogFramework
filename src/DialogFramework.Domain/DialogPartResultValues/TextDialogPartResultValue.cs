﻿namespace DialogFramework.Domain.DialogPartResultValues;

public record TextDialogPartResultValue : IDialogPartResultValue
{
    public TextDialogPartResultValue(string? value) => Value = value;
    public object? Value { get; }
    public ResultValueType ResultValueType => ResultValueType.Text;
}