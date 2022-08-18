namespace DialogFramework.Abstractions.DialogParts;

public interface IQuestionDialogPart : IGroupedDialogPart
{
    string Title { get; }
    IReadOnlyCollection<IDialogPartResultAnswerDefinition> Answers { get; }
    IReadOnlyCollection<IQuestionDialogPartValidator> Validators { get; }
    Result Validate(IDialog dialog, IDialogDefinition definition, IEnumerable<IDialogPartResultAnswer> answers);
}
