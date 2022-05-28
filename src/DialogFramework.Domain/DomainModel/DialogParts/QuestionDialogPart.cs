namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record QuestionDialogPart
{
    public IDialogPart? Validate(IDialogContext context,
                                 IDialog dialog,
                                 IConditionEvaluator conditionEvaluator,
                                 IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var errors = new List<IDialogValidationResult>();
        HandleValidate(context, dialog, dialogPartResults, errors);

        foreach (var validator in Validators)
        {
            errors.AddRange(validator.Validate(context, dialog, conditionEvaluator, dialogPartResults));
        }

        return errors.Count > 0
            ? new QuestionDialogPart(Title, Results, Validators, errors, Group, Heading, Id)
            : null;
    }

    public DialogState GetState() => DialogState.InProgress;

    protected virtual void HandleValidate(IDialogContext context,
                                          IDialog dialog,
                                          IEnumerable<IDialogPartResult> dialogPartResults,
                                          List<IDialogValidationResult> errors)
    {
        foreach (var dialogPartResult in dialogPartResults)
        {
            if (dialogPartResult.DialogPartId != Id)
            {
                errors.Add(new DialogValidationResult("Provided answer from wrong question", new ReadOnlyValueCollection<string>()));
                continue;
            }
            if (string.IsNullOrEmpty(dialogPartResult.ResultId))
            {
                continue;
            }
            var dialogPartResultDefinition = Results.SingleOrDefault(x => x.Id == dialogPartResult.ResultId);
            if (dialogPartResultDefinition == null)
            {
                errors.Add(new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]", new ReadOnlyValueCollection<string>()));
            }
            else
            {
                var resultValueType = dialogPartResultDefinition.ValueType;
                if (dialogPartResult.Value.ResultValueType != resultValueType)
                {
                    errors.Add(new DialogValidationResult($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered", new ReadOnlyValueCollection<string>()));
                }
            }
        }

        foreach (var dialogPartResultDefinition in Results)
        {
            var dialogPartResultsByPart = dialogPartResults
                .Where(x => x.DialogPartId == Id && x.ResultId == dialogPartResultDefinition.Id)
                .ToArray();
            errors.AddRange(dialogPartResultDefinition.Validate(context, dialog, this, dialogPartResultsByPart)
                                                      .Where(x => !string.IsNullOrEmpty(x.ErrorMessage)));
        }
    }
}
