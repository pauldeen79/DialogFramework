namespace DialogFramework.Domain.DialogPartResultAnswerDefinitionValidators;

public class ValueTypeValidator : IDialogPartResultAnswerDefinitionValidator
{
    public Type Type { get; }
    public ValueTypeValidator(Type type) => Type = type;

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IDialogPart part,
                                                         IDialogPartResultAnswerDefinition answerDefinition,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
    {
        if (answers.Any(x => x.Value.Value != null && !Type.IsInstanceOfType(x.Value.Value)))
        {
            yield return new DialogValidationResult($"Answer value of [{part.Id}.{answerDefinition.Id}] is not of type [{Type.FullName}]", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { answerDefinition.Id  }));
        }
    }
}
