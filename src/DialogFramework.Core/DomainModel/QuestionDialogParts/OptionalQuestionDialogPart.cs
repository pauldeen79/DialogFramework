namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record OptionalQuestionDialogPart : QuestionDialogPart
{
    public OptionalQuestionDialogPart(string id,
                                      string heading,
                                      string title,
                                      IDialogPartGroup group,
                                      IEnumerable<IDialogPartResultDefinition> results)
        : base(id, heading, title, group, results)
    {
    }
}
