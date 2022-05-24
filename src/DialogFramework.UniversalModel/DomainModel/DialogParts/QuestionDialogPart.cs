using System.Collections.Generic;
using System.Linq;
using CrossCutting.Common;
using CrossCutting.Common.Extensions;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record QuestionDialogPart
    {
        public IDialogPart? Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        {
            ValidationErrors.Clear();
            HandleValidate(context, dialog, dialogPartResults);

            foreach (var validator in Validators)
            {
                ValidationErrors.AddRange(validator.Validate(context, dialog, dialogPartResults));
            }

            return ValidationErrors.Count > 0
                ? this
                : null;
        }

        protected virtual void HandleValidate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
        {
            foreach (var dialogPartResult in dialogPartResults)
            {
                if (dialogPartResult.DialogPartId != Id)
                {
                    ValidationErrors.Add(new DialogValidationResult("Provided answer from wrong question", new ValueCollection<string>()));
                    continue;
                }
                if (string.IsNullOrEmpty(dialogPartResult.ResultId))
                {
                    continue;
                }
                var dialogPartResultDefinition = Results.SingleOrDefault(x => x.Id == dialogPartResult.ResultId);
                if (dialogPartResultDefinition == null)
                {
                    ValidationErrors.Add(new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]", new ValueCollection<string>()));
                }
                else
                {
                    var resultValueType = dialogPartResultDefinition.ValueType;
                    if (dialogPartResult.Value.ResultValueType != resultValueType)
                    {
                        ValidationErrors.Add(new DialogValidationResult($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered", new ValueCollection<string>()));
                    }
                }
            }

            foreach (var dialogPartResultDefinition in Results)
            {
                var dialogPartResultsByPart = dialogPartResults.Where(x => x.DialogPartId == Id && x.ResultId == dialogPartResultDefinition.Id).ToArray();
                ValidationErrors.AddRange(dialogPartResultDefinition.Validate(context, dialog, this, dialogPartResultsByPart)
                                                                    .Where(x => !string.IsNullOrEmpty(x.ErrorMessage)));
            }
        }
    }
}
