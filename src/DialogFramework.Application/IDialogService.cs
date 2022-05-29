namespace DialogFramework.Application;

public interface IDialogService
{
    bool CanStart(IDialogIdentifier dialogIdentifier);
    IDialogContext Start(IDialogIdentifier dialogIdentifier);
    bool CanContinue(IDialogContext context);
    IDialogContext Continue(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults);
    bool CanAbort(IDialogContext context);
    IDialogContext Abort(IDialogContext context);
    bool CanNavigateTo(IDialogContext context, string navigateToPartId);
    IDialogContext NavigateTo(IDialogContext context, string navigateToPartId);
    bool CanResetCurrentState(IDialogContext context);
    IDialogContext ResetCurrentState(IDialogContext context);
}
