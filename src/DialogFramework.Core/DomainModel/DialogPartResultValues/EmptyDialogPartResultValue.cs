namespace DialogFramework.Core.DomainModel.DialogPartResultValues;

public record EmptyDialogPartResultValue : IDialogPartResultValue
{
    public object? Value => null;
}
