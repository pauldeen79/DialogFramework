namespace DialogFramework.Application;

public interface IDialogService
{
    bool CanStart(IDialogDefinitionIdentifier dialogIdentifier);
    IDialog Start(IDialogDefinitionIdentifier dialogIdentifier);
    bool CanContinue(IDialog context, IEnumerable<IDialogPartResult> dialogPartResults);
    IDialog Continue(IDialog context, IEnumerable<IDialogPartResult> dialogPartResults);
    bool CanAbort(IDialog context);
    IDialog Abort(IDialog context);
    bool CanNavigateTo(IDialog context, IDialogPartIdentifier navigateToPartId);
    IDialog NavigateTo(IDialog context, IDialogPartIdentifier navigateToPartId);
    bool CanResetCurrentState(IDialog context);
    IDialog ResetCurrentState(IDialog context);
}
