namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record QuestionDialogPart : IQuestionDialogPart
{
    protected QuestionDialogPart(string id,
                                 string title,
                                 IDialogPartGroup group,
                                 IEnumerable<IQuestionDialogPartAnswer> answers)
    {
        Id = id;
        Title = title;
        Group = group;
        Answers = new ValueCollection<IQuestionDialogPartAnswer>(answers);
        ErrorMessages = new ValueCollection<string>();
    }

    public string Title { get; }
    public IDialogPartGroup Group { get; }
    public ValueCollection<IQuestionDialogPartAnswer> Answers { get; }
    public string Id { get; }
    public ValueCollection<string> ErrorMessages { get; protected set; }
    public DialogState State => DialogState.InProgress;
    public IDialogPart? Validate(IEnumerable<IProvidedAnswer> providedAnswers)
    {
        ErrorMessages.Clear();
        HandleValidate(providedAnswers);

        if (ErrorMessages.Any())
        {
            return this;
        }

        return null;
    }

    protected virtual void HandleValidate(IEnumerable<IProvidedAnswer> providedAnswers)
    {
        foreach (var providedAnswer in providedAnswers)
        {
            if (providedAnswer.Question.Id == Id)
            {
                var validationContext = new ValidationContext(providedAnswer);
                foreach (var validationError in providedAnswer.Validate(validationContext).Where(x => !string.IsNullOrEmpty(x.ErrorMessage)))
                {
                    ErrorMessages.Add(validationError.ErrorMessage!);
                }
            }
            else
            {
                ErrorMessages.Add("Provided answer from wrong question");
            }
        }
    }
}
