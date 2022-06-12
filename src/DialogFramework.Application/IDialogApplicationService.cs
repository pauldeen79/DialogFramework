namespace DialogFramework.Application;

public interface IDialogApplicationService
{
    Result<IDialog> Start(IDialogDefinitionIdentifier dialogDefinitionIdentifier);
    Result<IDialog> Continue(IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults);
    Result<IDialog> Abort(IDialog dialog);
    Result<IDialog> NavigateTo(IDialog dialog, IDialogPartIdentifier navigateToPartId);
    Result<IDialog>ResetCurrentState(IDialog dialog);
}
