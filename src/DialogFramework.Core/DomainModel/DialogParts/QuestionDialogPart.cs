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
            if (dialogPartResult.DialogPart.Id != Id)
            {
                ErrorMessages.Add("Provided answer from wrong question");
                continue;
            }

            var resultValueType = dialogPartResult.DialogPart.GetResultValueType(dialogPartResult);
            if (resultValueType != null && dialogPartResult.Value.ResultValueType != resultValueType.Value)
            {
                ErrorMessages.Add($"Result should be of type [{resultValueType.Value}], but type [{dialogPartResult.Value.ResultValueType}] was answered");
            }

            var validationContext = new ValidationContext(dialogPartResult);
            ErrorMessages.AddRange(dialogPartResult.Validate(validationContext)
                                                   .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                                                   .Select(x => x.ErrorMessage));
        }
    }
}
