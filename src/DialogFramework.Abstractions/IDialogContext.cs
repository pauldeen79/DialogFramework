namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    string Id { get; }
    IDialogIdentifier CurrentDialogIdentifier { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }

    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialog dialog, IDialogPart firstPart);

    IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults);

    bool CanContinue(IDialog dialog);
    IDialogContext Continue(IDialog dialog, IDialogPart nextPart);

    bool CanAbort(IDialog dialog);
    IDialogContext Abort(IDialog dialog);

    IDialogContext Error(IDialog dialog, Exception? exception);

    bool CanNavigateTo(IDialog dialog, IDialogPart navigateToPart);
    IDialogContext NavigateTo(IDialog dialog, IDialogPart navigateToPart);

    bool CanResetCurrentState(IDialog dialog);
    IDialogContext ResetCurrentState(IDialog dialog);
}
