using System;
using System.Collections.Generic;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel
{
    public partial record DialogPartResultDefinition
    {
        public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                             IDialogPart dialogPart,
                                                             IEnumerable<IDialogPartResult> dialogPartResults)
        {
            throw new NotImplementedException();
        }
    }
}
