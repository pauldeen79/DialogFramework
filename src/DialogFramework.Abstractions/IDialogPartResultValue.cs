namespace DialogFramework.Abstractions;

public interface IDialogPartResultValue : IDialogPartResultValueAnswer
{
    ResultValueType ResultValueType { get; }
}
