using System;
using System.Collections.Generic;
using System.Linq;
using CrossCutting.Common;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.UniversalModel
{
    public partial record DialogContext
    {
        public IDialogContext Abort(IAbortedDialogPart abortDialogPart)
            => new DialogContext(Id, CurrentDialog, abortDialogPart, (abortDialogPart as IGroupedDialogPart)?.Group, DialogState.Aborted, Answers, null);

        public IDialogContext AddDialogPartResults(IEnumerable<IDialogPartResult> dialogPartResults)
            => new DialogContext(Id, CurrentDialog, CurrentPart, (CurrentPart as IGroupedDialogPart)?.Group, CurrentState, new ValueCollection<IDialogPartResult>(CurrentDialog.ReplaceAnswers(Answers, dialogPartResults)), null);

        public IDialogContext Continue(IDialogPart nextPart, DialogState state)
            => new DialogContext(Id, CurrentDialog, nextPart, (nextPart as IGroupedDialogPart)?.Group, state, new ValueCollection<IDialogPartResult>(Answers), null);

        public IDialogContext Error(IErrorDialogPart errorDialogPart, Exception ex)
            => new DialogContext(Id, CurrentDialog, errorDialogPart, (errorDialogPart as IGroupedDialogPart)?.Group, DialogState.ErrorOccured, Answers, ex);

        public bool CanStart()
           => CurrentState == DialogState.Initial && CurrentDialog.Metadata.CanStart;

        public IDialogContext Start(IDialogPart firstPart)
            => new DialogContext(Id, CurrentDialog, firstPart, (firstPart as IGroupedDialogPart)?.Group, firstPart.State, new ValueCollection<IDialogPartResult>(), null);

        public bool CanNavigateTo(IDialogPart navigateToPart)
            => CurrentDialog.CanNavigateTo(CurrentPart, navigateToPart, Answers);

        public IDialogContext NavigateTo(IDialogPart navigateToPart)
            => new DialogContext(Id, CurrentDialog, navigateToPart, (navigateToPart as IGroupedDialogPart)?.Group, navigateToPart.State, Answers, null);

        public IEnumerable<IDialogPartResult> GetDialogPartResultsByPart(IDialogPart dialogPart)
            => Answers.Where(x => x.DialogPartId == dialogPart.Id);

        public IEnumerable<IDialogPartResult> GetAllDialogPartResults() => Answers;

        public IDialogContext ResetDialogPartResultByPart(IDialogPart dialogPart)
            => new DialogContext(Id, CurrentDialog, CurrentPart, (CurrentPart as IGroupedDialogPart)?.Group, CurrentState, new ValueCollection<IDialogPartResult>(CurrentDialog.ResetDialogPartResultByPart(Answers, CurrentPart)), Exception);
    }
}
