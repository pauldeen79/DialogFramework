namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialog CurrentDialog { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    IDialogContext Start(IDialogPart firstPart);
    IDialogContext ProvideAnswers(IEnumerable<IProvidedAnswer> providedAnswers);
    IDialogContext Continue(IDialogPart nextPart, DialogState state);
    IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
    bool CanNavigateTo(IDialogPart navigateToPart);
    IDialogContext NavigateTo(IDialogPart navigateToPart);
    IProvidedAnswer? GetProvidedAnswerByPart(IDialogPart dialogPart);
    IDialogContext ResetProvidedAnswerByPart(IDialogPart dialogPart);
}
