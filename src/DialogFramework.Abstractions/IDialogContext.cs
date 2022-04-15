namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialog CurrentDialog { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    IDialogContext Start(IDialogPart firstPart);
    IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults);
    IDialogContext Continue(IDialogPart nextPart, DialogState state);
    IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
    bool CanNavigateTo(IDialogPart navigateToPart);
    IDialogContext NavigateTo(IDialogPart navigateToPart);
    IDialogPartResult? GetDialogPartResultByPart(IDialogPart dialogPart);
    IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart);
}
