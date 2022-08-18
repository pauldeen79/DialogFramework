namespace DialogFramework.Abstractions;

public interface IDialogPartResultAnswerDefinition
{
    IDialogPartResultIdentifier Id { get; }
    string Title { get; }
    ResultValueType ValueType { get; }
    IReadOnlyCollection<IDialogPartResultAnswerDefinitionValidator> Validators { get; }

    IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                  IDialogDefinition definition,
                                                  IDialogPart part,
                                                  IEnumerable<IDialogPartResultAnswer> answers);
}
