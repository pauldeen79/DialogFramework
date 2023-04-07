namespace DialogFramework.Domain.Contracts;

public interface IValidatableDialogPart
{
    Result Validate<T>(T value, Dialog dialog);
}
