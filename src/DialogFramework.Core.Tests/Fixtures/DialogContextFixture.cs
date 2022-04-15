namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialog currentDialog)
        : base(currentDialog)
    {
    }

    public DialogContextFixture(IDialog currentDialog, IDialogPart currentPart, DialogState currentState)
        : base(currentDialog, currentPart, currentState)
    {
    }
}
