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

            if (!string.IsNullOrEmpty(dialogPartResult.ResultId))
            {
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
                        ErrorMessages.Add($"Result for [{Id}] should be of type [{resultValueType}], but type [{dialogPartResult.Value.ResultValueType}] was answered");
                    }

                    var validationContext = new ValidationContext(this);
                    ErrorMessages.AddRange(dialogPartResult.Validate(validationContext)
                                                           .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                                                           .Select(x => x.ErrorMessage));
                }
            }
        }
    }
}
