using System.Collections.Generic;

namespace DialogFramework.Abstractions.DomainModel
{
    public interface IDialogPartResultDefinitionValidator
    {
        IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                      IDialogPart dialogPart,
                                                      IDialogPartResultDefinition dialogPartResultDefinition,
                                                      IEnumerable<IDialogPartResult> dialogPartResults);
    }
}
