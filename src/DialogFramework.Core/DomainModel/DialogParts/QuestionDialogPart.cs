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
        ErrorMessages = new ValueCollection<string>();
    }

    public string Title { get; }
    public string Heading { get; }
    public IDialogPartGroup Group { get; }
    public ValueCollection<IDialogPartResultDefinition> Results { get; }
    public string Id { get; }
    public ValueCollection<string> ErrorMessages { get; }
    public DialogState State => DialogState.InProgress;
    public IDialogPart? Validate(IEnumerable<IDialogPartResult> dialogPartResults)
    {
        ErrorMessages.Clear();
        HandleValidate(dialogPartResults);

        return ErrorMessages.Count > 0
            ? this
            : null;
    }

    protected virtual void HandleValidate(IEnumerable<IDialogPartResult> dialogPartResults)
    {
        foreach (var dialogPartResult in dialogPartResults)
        {
            if (dialogPartResult.DialogPartId != Id)
            {
                ErrorMessages.Add("Provided answer from wrong question");
                continue;
            }
            if (string.IsNullOrEmpty(dialogPartResult.ResultId))
            {
                continue;
            }
            var dialogPartResultDefinition = Results.SingleOrDefault(x => x.Id == dialogPartResult.ResultId);
            if (dialogPartResultDefinition == null)
            {
                ErrorMessages.Add($"Unknown Result Id: [{dialogPartResult.ResultId}]");
            }
            else
            {
                var resultValueType = dialogPartResultDefinition.ValueType;
                if (dialogPartResult.Value.ResultValueType != resultValueType)
                {
                    ErrorMessages.Add($"Result for [{dialogPartResult.DialogPartId}.{dialogPartResult.ResultId}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered");
                }
            }
        }

        foreach (var dialogPartResultDefinition in Results)
        {
            var dialogPartResultsByPart = dialogPartResults.Where(x => x.DialogPartId == Id && x.ResultId == dialogPartResultDefinition.Id).ToArray();
            var validationContext = new ValidationContext(this);
            ErrorMessages.AddRange(dialogPartResultDefinition.Validate(validationContext, this, dialogPartResultsByPart)
                                                             .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                                                             .Select(x => x.ErrorMessage));
        }
    }
}
