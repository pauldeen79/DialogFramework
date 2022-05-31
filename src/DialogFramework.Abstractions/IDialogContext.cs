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
    IReadOnlyCollection<IError> Errors { get; }

    bool CanStart(IDialog dialog);
    void Start(IDialog dialog, IDialogPartIdentifier firstPartId);

    void AddDialogPartResults(IDialog dialog, IEnumerable<IDialogPartResult> partResults);

    bool CanContinue(IDialog dialog);
    void Continue(IDialog dialog, IDialogPartIdentifier nextPartId, IEnumerable<IDialogValidationResult> validationResults);

    bool CanAbort(IDialog dialog);
    void Abort(IDialog dialog);

    void Error(IDialog dialog, IEnumerable<IError> errors);

    bool CanNavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);
    void NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);

    bool CanResetCurrentState(IDialog dialog);
    void ResetCurrentState(IDialog dialog);
}
