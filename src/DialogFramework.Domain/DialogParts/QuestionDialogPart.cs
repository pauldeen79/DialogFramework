namespace DialogFramework.Domain.DialogParts;

public partial record QuestionDialogPart : IValidatableObject
{
    public Result Validate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> answers)
    {
        var errors = new List<IDialogValidationResult>(HandleValidate(dialog, definition, answers));

        foreach (var validator in Validators)
        {
            errors.AddRange(validator.Validate(dialog, definition, answers));
        }

        return errors.Count > 0
            ? Result.Invalid("Validation failed, see ValidationErrors for more details", errors.Select(x => new ValidationError(x.ErrorMessage, x.PartResultIds.Select(y => y.Value))))
            : Result.Success();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateIds = Answers
            .GroupBy(x => x.Id)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (!Answers.Any())
        {
            yield return new ValidationResult("At least one result is required for a question dialog part", new[] { nameof(Answers) });
        }

        if (duplicateIds.Any())
        {
            yield return new ValidationResult($"Result Ids should be unique. Non unique ids: {string.Join(", ", duplicateIds.Select(x => x.ToString()))}");
        }
    }

    public override IDialogPartBuilder CreateBuilder() => new QuestionDialogPartBuilder(this);
    public override bool SupportsReset() => true;

    protected virtual IEnumerable<IDialogValidationResult> HandleValidate(IDialog dialog,
                                                                          IDialogDefinition definition,
                                                                          IEnumerable<IDialogPartResultAnswer> answers)
    {
        var unknownResultIds = answers
            .Select(x => x.ResultId)
            .Where(dialogPartResultId => !string.IsNullOrEmpty(dialogPartResultId.Value)
                && !Answers.Any(x => Equals(x.Id, dialogPartResultId)));

        foreach (var dialogPartResultId in unknownResultIds)
        {
            yield return new DialogValidationResult($"Unknown Result Id: [{dialogPartResultId}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>());
        }

        foreach (var dialogPartResultDefinition in Answers)
        {
            var dialogPartResultsByPart = answers
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
