namespace DialogFramework.Domain.Contracts;

public interface IDialogSubmitter
{
    bool SupportsDialog(string id, string version);
    Result<Dialog> Submit(Dialog dialog);
}
