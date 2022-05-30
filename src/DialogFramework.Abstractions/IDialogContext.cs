namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialogContextIdentifier Id { get; }
    IDialogIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }

    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialog dialog, IDialogPartIdentifier firstPartId);

    IDialogContext AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults);

    bool CanContinue(IDialog dialog);
    IDialogContext Continue(IDialog dialog, IDialogPartIdentifier nextPartId, IEnumerable<IDialogValidationResult> validationResults);

    bool CanAbort(IDialog dialog);
    IDialogContext Abort(IDialog dialog);

    IDialogContext Error(IDialog dialog, string? errorMessage);

    bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);
    IDialogContext NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);

    bool CanResetCurrentState(IDialog dialog);
    IDialogContext ResetCurrentState(IDialog dialog);
}
