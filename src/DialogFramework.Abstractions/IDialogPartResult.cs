﻿namespace DialogFramework.Abstractions;

public interface IDialogPartResult 
{
    IDialogPartIdentifier DialogPartId { get; }
    IDialogPartResultIdentifier ResultId { get; }
    IDialogPartResultValueAnswer Value { get; }
}
