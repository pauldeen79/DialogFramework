using System;
using CrossCutting.Common;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.Domains;
using DialogFramework.UniversalModel.DomainModel;

namespace DialogFramework.UniversalModel
{
    public class DialogContextFactory : IDialogContextFactory
    {
        public bool CanCreate(IDialog dialog)
        {
            return dialog is Dialog;
        }

        public IDialogContext Create(IDialog dialog)
        {
            return new DialogContext(Guid.NewGuid().ToString(), dialog, new EmptyDialogPart(), null, DialogState.Initial, new ValueCollection<IDialogPartResult>(), null);
        }

        private sealed class EmptyDialogPart : IDialogPart
        {
            public string Id => "Empty";
            public DialogState State => DialogState.Initial;
        }
    }
}
