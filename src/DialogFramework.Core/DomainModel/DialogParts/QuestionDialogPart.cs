namespace DialogFramework.Core.DomainModel.DialogParts;

public record QuestionDialogPart : IQuestionDialogPart
{
    public QuestionDialogPart(string id,
                              string heading,
                              string title,
                              IDialogPartGroup group,
                              IEnumerable<IDialogPartResultDefinition> results)
    {
        Id = id;
        Heading = heading;
        Title = title;
        Group = group;
        Results = new ValueCollection<IDialogPartResultDefinition>(results);
        ValidationErrors = new ValueCollection<IDialogValidationResult>();
    }

    public string Title { get; }
    public string Heading { get; }
    public IDialogPartGroup Group { get; }
    public ValueCollection<IDialogPartResultDefinition> Results { get; }
    public string Id { get; }
    public ValueCollection<IDialogValidationResult> ValidationErrors { get; }
    public DialogState State => DialogState.InProgress;
    public IDialogPart? Validate(IDialogContext context, IDialog dialog, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        ValidationErrors.Clear();
        HandleValidate(context, dialog, dialogPartResults);

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
                ValidationErrors.Add(new DialogValidationResult("Provided answer from wrong question"));
                continue;
            }
            if (string.IsNullOrEmpty(dialogPartResult.ResultId))
            {
                continue;
            }
            var dialogPartResultDefinition = Results.SingleOrDefault(x => x.Id == dialogPartResult.ResultId);
            if (dialogPartResultDefinition == null)
            {
                ValidationErrors.Add(new DialogValidationResult($"Unknown Result Id: [{dialogPartResult.ResultId}]"));
            }
            else
            {
                var resultValueType = dialogPartResultDefinition.ValueType;
                if (dialogPartResult.Value.ResultValueType != resultValueType)
                {
                    ValidationErrors.Add(new DialogValidationResult($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered"));
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
