namespace DialogFramework.Domain.Contracts;

public interface IDialogSubmitter
{
    bool SupportsDialog(string id, Version version);
    Result<Dialog> Submit(Dialog dialog);
}
