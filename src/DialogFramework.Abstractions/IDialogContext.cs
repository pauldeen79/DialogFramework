using System;

namespace DialogFramework.Abstractions;

public interface IDialogContext
{
    IDialog CurrentDialog { get; }
    IDialogPart CurrentPart { get; }
    IDialogPartGroup? CurrentGroup { get; }
    DialogState CurrentState { get; }
    IDialogContext Abort(IAbortedDialogPart abortDialogPart);
    IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
    IDialogContext Continue(IEnumerable<IProvidedAnswer> providedAnswers, IDialogPart nextPart, IDialogPartGroup? nextGroup, DialogState state);
    IDialogContext Start(IDialogPart firstPart, IDialogPartGroup? firstGroup);
}
