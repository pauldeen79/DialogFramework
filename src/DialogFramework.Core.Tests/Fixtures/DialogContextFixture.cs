namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialogIdentifier currentDialogIdentifier)
        : base(currentDialogIdentifier)
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialogIdentifier, currentPart, currentState)
    {
    }

    public void AddAnswer(IDialogPartResult result) => Answers.Add(result);
}
