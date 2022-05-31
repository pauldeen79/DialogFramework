namespace DialogFramework.Domain.DialogParts;

public partial record QuestionDialogPart
{
    public IDialogPart? Validate(IDialogContext context,
                                 IDialog dialog,
                                 IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var errors = new List<IDialogValidationResult>();
        HandleValidate(context, dialog, dialogPartResults, errors);

        foreach (var validator in Validators)
        {
            errors.AddRange(validator.Validate(context, dialog, dialogPartResults));
        }

        return errors.Count > 0
            ? new QuestionDialogPart(Title, Results, Validators, errors, Group, Heading, Id)
            : null;
    }

    public DialogState GetState() => DialogState.InProgress;

    public IDialogPartBuilder CreateBuilder() => new QuestionDialogPartBuilder(this);

    protected virtual void HandleValidate(IDialogContext context,
                                          IDialog dialog,
                                          IEnumerable<IDialogPartResult> dialogPartResults,
                                          List<IDialogValidationResult> errors)
    {
        foreach (var dialogPartResult in dialogPartResults)
        {
            if (!Equals(dialogPartResult.DialogPartId, Id))
            {
                errors.Add(new DialogValidationResult("Provided answer from wrong question", new ReadOnlyValueCollection<IDialogPartResultIdentifier>()));
                continue;
            }
            if (string.IsNullOrEmpty(dialogPartResult.ResultId.Value))
            {
                continue;
            }
            var dialogPartResultDefinition = Results.SingleOrDefault(x => Equals(x.Id, dialogPartResult.ResultId));
            if (dialogPartResultDefinition == null)
            {
                errors.Add(new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>()));
            }
            else
            {
                var resultValueType = dialogPartResultDefinition.ValueType;
                if (dialogPartResult.Value.ResultValueType != resultValueType)
                {
                    errors.Add(new DialogValidationResult($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered", new ReadOnlyValueCollection<IDialogPartResultIdentifier>()));
                }
            }
        }

        foreach (var dialogPartResultDefinition in Results)
        {
            var dialogPartResultsByPart = dialogPartResults
                .Where(x => Equals(x.DialogPartId, Id) && Equals(x.ResultId, dialogPartResultDefinition.Id))
                .ToArray();
            errors.AddRange(dialogPartResultDefinition.Validate(context, dialog, this, dialogPartResultsByPart)
                                                      .Where(x => !string.IsNullOrEmpty(x.ErrorMessage)));
        }
    }
}
