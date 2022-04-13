using System;

namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialog CurrentDialog { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    IDialogContext Start(IDialogPart firstPart);
    IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, DialogState state);
    IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
}
