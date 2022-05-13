namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialog currentDialog)
        : base(currentDialog)
    {
    }

    public DialogContextFixture(string id, IDialog currentDialog, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialog, currentPart, currentState)
    {
    }

    public void AddAnswer(IDialogPartResult result) => Answers.Add(result);
}
