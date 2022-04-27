namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultValue
{
    object? Value { get; }
    ResultValueType ResultValueType { get; }
}
