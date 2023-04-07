namespace DialogFramework.Application;

public class DialogService : IDialogService
{
    private readonly IEnumerable<IDialogSubmitter> _submitters;

    public DialogService(IEnumerable<IDialogSubmitter> submitters)
    {
        _submitters = submitters;
    }

    public Result<Dialog> Submit(Dialog dialog)
    {
        var supportedSubmitters = _submitters.Where(x => x.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion)).ToArray();
        if (supportedSubmitters.Length == 0)
        {
            return Result<Dialog>.NotSupported($"The dialog definition Id [{dialog.DefinitionId}], version [{dialog.DefinitionVersion}] is not supported");
        }

        // In case multiple submitters are found, take the last one.
        // Best way is to first register most generic submitters, then more specific, and last the one to indicate the dialog is not supported.
        var submitter = supportedSubmitters.Last();

        return submitter.Submit(dialog);
    }
}
