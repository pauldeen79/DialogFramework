namespace DialogFramework.Abstractions;

public interface IDialogPartResultValue
{
    object? Value { get; }
    ResultValueType ResultValueType { get; }
}
