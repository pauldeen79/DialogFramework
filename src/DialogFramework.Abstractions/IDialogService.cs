﻿namespace DialogFramework.Abstractions;

public interface IDialogService
{
    bool CanStart(IDialog dialog);
    IDialogContext Start(IDialog dialog);
    bool CanContinue(IDialogContext context);
    IDialogContext Continue(IDialogContext context, IEnumerable<IProvidedAnswer> providedAnswers);
    bool CanAbort(IDialogContext context);
    IDialogContext Abort(IDialogContext context);
}
