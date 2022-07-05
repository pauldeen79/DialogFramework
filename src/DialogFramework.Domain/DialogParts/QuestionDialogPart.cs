namespace DialogFramework.Domain.DialogParts;

public partial record QuestionDialogPart : DialogPartBase, IValidatableObject
{
    public Result Validate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> results)
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

    protected virtual IEnumerable<IDialogValidationResult> HandleValidate(IDialog dialog,
                                                                          IDialogDefinition definition,
                                                                          IEnumerable<IDialogPartResultAnswer> results)
    {
        var unknownResultIds = results
            .Select(x => x.ResultId)
            .Where(dialogPartResultId => !string.IsNullOrEmpty(dialogPartResultId.Value)
                && !Results.Any(x => Equals(x.Id, dialogPartResultId)));

        foreach (var dialogPartResultId in unknownResultIds)
        {
            yield return new DialogValidationResult($"Unknown Result Id: [{dialogPartResultId}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }

        foreach (var dialogPartResultDefinition in Results)
        {
            var dialogPartResultsByPart = results
                .Where(x => Equals(x.ResultId, dialogPartResultDefinition.Id))
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
