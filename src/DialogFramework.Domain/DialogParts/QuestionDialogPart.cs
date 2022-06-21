namespace DialogFramework.Domain.DialogParts;

public partial record QuestionDialogPart : IValidatableObject
{
    public Result Validate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResult> results)
    {
        var errors = new List<IDialogValidationResult>(HandleValidate(dialog, definition, results));

        foreach (var validator in Validators)
        {
            errors.AddRange(validator.Validate(dialog, definition, results));
        }

        return errors.Count > 0
            ? Result.Invalid("Validation failed, see ValidationErrors for more details", errors.Select(x => new ValidationError(x.ErrorMessage, x.DialogPartResultIds.Select(y => y.Value))))
            : Result.Success();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateIds = Results
            .GroupBy(x => x.Id)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (!Results.Any())
        {
            yield return new ValidationResult("At least one result is required for a question dialog part", new[] { nameof(Results) });
        }

        if (duplicateIds.Any())
        {
            yield return new ValidationResult($"Result Ids should be unique. Non unique ids: {string.Join(", ", duplicateIds.Select(x => x.ToString()))}");
        }
    }

    public DialogState GetState() => DialogState.InProgress;

    public IDialogPartBuilder CreateBuilder() => new QuestionDialogPartBuilder(this);

    public bool SupportsReset() => true;

    protected virtual IEnumerable<IDialogValidationResult> HandleValidate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResult> results)
    {
        foreach (var dialogPartResult in results)
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
            var currentPartResults = Results.Where(x => Equals(x.Id, dialogPartResult.ResultId)).ToArray();
            if (currentPartResults.Length == 0)
            {
                yield return new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
            }
            var errors = from dialogPartResultDefinition in currentPartResults
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
            var dialogPartResultsByPart = results
                .Where(x => Equals(x.DialogPartId, Id) && Equals(x.ResultId, dialogPartResultDefinition.Id))
                .ToArray();
            var errors = dialogPartResultDefinition.Validate(dialog, definition, this, dialogPartResultsByPart)
                                                   .Where(x => !string.IsNullOrEmpty(x.ErrorMessage));
            foreach (var error in errors)
            {
                yield return error;
            }
        }
    }
}
