namespace DialogFramework.Domain.DialogParts;

public partial record QuestionDialogPart : IValidatableObject
{
    public IDialogPart? Validate(IDialog dialog,
                                 IDialogDefinition dialogDefinition,
                                 IEnumerable<IDialogPartResult> dialogPartResults)
    {
        var errors = new List<IDialogValidationResult>(HandleValidate(dialog, dialogDefinition, dialogPartResults));

        foreach (var validator in Validators)
        {
            errors.AddRange(validator.Validate(dialog, dialogDefinition, dialogPartResults));
        }

        return errors.Count > 0
            ? new QuestionDialogPart(Title, Results, Validators, errors, Group, Heading, Id)
            : null;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateIds = Results
            .GroupBy(x => x.Id)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicateIds.Any())
        {
            yield return new ValidationResult($"Result Ids should be unique. Non unique ids: {string.Join(", ", duplicateIds.Select(x => x.ToString()))}");
        }
    }

    public DialogState GetState() => DialogState.InProgress;

    public IDialogPartBuilder CreateBuilder() => new QuestionDialogPartBuilder(this);

    protected virtual IEnumerable<IDialogValidationResult> HandleValidate(IDialog dialog,
                                                                          IDialogDefinition dialogDefinition,
                                                                          IEnumerable<IDialogPartResult> dialogPartResults)
    {
        foreach (var dialogPartResult in dialogPartResults)
        {
            if (!Equals(dialogPartResult.DialogPartId, Id))
            {
                yield return new DialogValidationResult("Provided answer from wrong question", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
                continue;
            }
            if (string.IsNullOrEmpty(dialogPartResult.ResultId.Value))
            {
                continue;
            }
            var results = Results.Where(x => Equals(x.Id, dialogPartResult.ResultId)).ToArray();
            if (results.Length == 0)
            {
                yield return new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
            }
            var errors = from dialogPartResultDefinition in results
                let resultValueType = dialogPartResultDefinition.ValueType
                where dialogPartResult.Value.ResultValueType != resultValueType
                select new DialogValidationResult($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
            foreach (var error in errors)
            {
                yield return error;
            }
        }

        foreach (var dialogPartResultDefinition in Results)
        {
            var dialogPartResultsByPart = dialogPartResults
                .Where(x => Equals(x.DialogPartId, Id) && Equals(x.ResultId, dialogPartResultDefinition.Id))
                .ToArray();
            var errors = dialogPartResultDefinition.Validate(dialog, dialogDefinition, this, dialogPartResultsByPart)
                                                   .Where(x => !string.IsNullOrEmpty(x.ErrorMessage));
            foreach (var error in errors)
            {
                yield return error;
            }
        }
    }
}
