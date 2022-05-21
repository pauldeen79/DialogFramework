using System;
using DialogFramework.Abstractions;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record DecisionDialogPart
    {
        public string GetNextPartId(IDialogContext context)
        {
            throw new NotImplementedException();
        }
    }
}
