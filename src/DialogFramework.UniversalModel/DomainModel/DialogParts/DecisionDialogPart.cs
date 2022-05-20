using System;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record DecisionDialogPart
    {
        public IDialogPart GetNextPart(IDialogContext context)
        {
            throw new NotImplementedException();
        }
    }
}
