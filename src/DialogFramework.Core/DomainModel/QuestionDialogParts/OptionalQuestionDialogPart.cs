namespace DialogFramework.Core.DomainModel.QuestionDialogParts;

public record OptionalQuestionDialogPart : QuestionDialogPart
{
    public OptionalQuestionDialogPart(string id,
                                      string heading,
                                      string message,
                                      IDialogPartGroup group,
                                      IEnumerable<IDialogPartResultDefinition> results)
        : base(id, heading, message, group, results)
    {
    }
}
