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
    }

    public string Title { get; }
    public IDialogPartGroup Group { get; }
    public ValueCollection<IQuestionDialogPartAnswer> Answers { get; }
    public string Id { get; }
    public string? ErrorMessage { get; protected set; }
    public DialogState State => DialogState.InProgress;
    public abstract IDialogPart? Validate(IEnumerable<IProvidedAnswer> providedAnswers);
}
