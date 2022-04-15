﻿namespace DialogFramework.Abstractions;

public interface IDialogService
{
    IDialogContext Start(IDialog dialog);
    bool CanContinue(IDialogContext context);
    IDialogContext Continue(IDialogContext context, IEnumerable<IProvidedAnswer> providedAnswers);
    bool CanAbort(IDialogContext context);
    IDialogContext Abort(IDialogContext context);
    bool CanNavigateTo(IDialogContext context, IDialogPart navigateToPart);
    IDialogContext NavigateTo(IDialogContext context, IDialogPart navigateToPart);
}
