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
            foreach (var validator in Validators)
            {
                foreach (var validationError in validator.Validate(context, dialogPart, this, dialogPartResults))
                {
                    yield return validationError;
                }
            }
        }
    }
}
