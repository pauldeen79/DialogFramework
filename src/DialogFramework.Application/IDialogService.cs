namespace DialogFramework.Application;

public interface IDialogService
{
    bool CanStart(IDialogDefinitionIdentifier dialogDefinitionIdentifier);
    IDialog Start(IDialogDefinitionIdentifier dialogDefinitionIdentifier);
    bool CanContinue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    IDialog Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    bool CanAbort(IDialog dialog);
    IDialog Abort(IDialog dialog);
    bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);
    IDialog NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);
    bool CanResetCurrentState(IDialog dialog);
    IDialog ResetCurrentState(IDialog dialog);
}
