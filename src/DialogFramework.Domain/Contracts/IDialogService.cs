namespace DialogFramework.Domain.Contracts;

public interface IDialogService
{
    Result<Dialog> Submit(Dialog dialog);
}
