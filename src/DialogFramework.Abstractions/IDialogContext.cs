using System;
using System.Collections.Generic;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.Abstractions
{
    public interface IDialogContext
    {
        string Id { get; }
        IDialog CurrentDialog { get; }
        IDialogPart CurrentPart { get; }
        IDialogPartGroup? CurrentGroup { get; }
        DialogState CurrentState { get; }
        bool CanStart();
        IDialogContext Start(IDialogPart firstPart);
        IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults);
        IDialogContext Continue(IDialogPart nextPart, DialogState state);
        IDialogContext Abort(IAbortedDialogPart abortDialogPart);
        IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex);
        bool CanNavigateTo(IDialogPart navigateToPart);
        IDialogContext NavigateTo(IDialogPart navigateToPart);
        IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart);
        IEnumerable<IDialogPartResult> GetAllDialogPartResults();
        IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart);
    }
}
