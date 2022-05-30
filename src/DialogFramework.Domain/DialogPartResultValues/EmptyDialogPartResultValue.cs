﻿namespace DialogFramework.Domain.DialogPartResultValues;

public record EmptyDialogPartResultValue : IDialogPartResultValue
{
    public object? Value => null;
    public ResultValueType ResultValueType => ResultValueType.None;
}