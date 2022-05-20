using System;
using System.Collections.Generic;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record QuestionDialogPart
    {
        public IDialogPart? Validate(IDialogContext context, IEnumerable<IDialogPartResult> dialogPartResults)
        {
            throw new NotImplementedException();
        }
    }
}
