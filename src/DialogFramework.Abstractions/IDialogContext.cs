namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    string Id { get; }
    IDialogIdentifier CurrentDialogIdentifier { get; }
    string CurrentPartId { get; }
    string? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }

    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialog dialog, string firstPartId);

    IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults);

    bool CanContinue(IDialog dialog);
    IDialogContext Continue(IDialog dialog, string nextPartId, IEnumerable<IDialogValidationResult> validationResults);

    bool CanAbort(IDialog dialog);
    IDialogContext Abort(IDialog dialog);

    IDialogContext Error(IDialog dialog, Exception? exception);

    bool CanNavigateTo(IDialog dialog, string navigateToPartId);
    IDialogContext NavigateTo(IDialog dialog, string navigateToPartId);

    bool CanResetCurrentState(IDialog dialog);
    IDialogContext ResetCurrentState(IDialog dialog);
}
