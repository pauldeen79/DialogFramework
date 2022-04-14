﻿using System;

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
}
