namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    string Id { get; }
    IDialogIdentifier CurrentDialogIdentifier { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialogPart firstPart);
    IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults, IDialog dialog);
    IDialogContext Continue(IDialogPart nextPart, DialogState state);
    IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
    bool CanNavigateTo(IDialogPart navigateToPart, IDialog dialog);
    IDialogContext NavigateTo(IDialogPart navigateToPart);
    IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart);
    IEnumerable<IDialogPartResult> GetAllDialogPartResults();
    IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart, IDialog dialog);
}
