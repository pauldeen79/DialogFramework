using System;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record ErrorDialogPart
    {
        public IErrorDialogPart ForException(Exception ex)
            => new ErrorDialogPart(ErrorMessage, ex, Id, DialogState.ErrorOccured);
    }
}
