namespace DialogFramework.Abstractions;

public interface IDialogService
{
    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialog dialog);
    bool CanContinue(IDialogContext context);
    IDialogContext Continue(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults);
    bool CanAbort(IDialogContext context);
    IDialogContext Abort(IDialogContext context);
    bool CanNavigateTo(IDialogContext context, IDialogPart navigateToPart);
    IDialogContext NavigateTo(IDialogContext context, IDialogPart navigateToPart);
}
