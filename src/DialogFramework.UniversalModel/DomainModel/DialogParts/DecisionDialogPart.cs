using System;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record DecisionDialogPart
    {
        public string GetNextPartId(IDialogContext context, IDialog dialog)
        {
            throw new NotImplementedException();
        }
    }
}
