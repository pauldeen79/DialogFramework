namespace DialogFramework.Domain;

public partial record BeforeNavigateArguments
{
    public BeforeNavigateArguments(IDialog dialog)
    {
        if (dialog == null)
        {
            throw new ArgumentNullException(nameof(dialog));
        }

        CurrentDialogId = dialog.Id;
        CurrentDialogIdentifier = dialog.CurrentDialogIdentifier;
        CurrentGroupId = dialog.CurrentGroupId;
        CurrentPartId = dialog.CurrentPartId;
        CurrentState = dialog.CurrentState;
        ErrorMessage = dialog.ErrorMessage;
    }
}
