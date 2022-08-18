namespace DialogFramework.Domain;

public partial record DialogPartResultAnswerDefinition
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IDialogPart dialogPart,
                                                         IEnumerable<IDialogPartResultAnswer> dialogPartResults)
    {
        foreach (var validator in Validators)
        {
            foreach (var validationError in validator.Validate(dialog, dialogDefinition, dialogPart, this, dialogPartResults))
            {
                yield return validationError;
            }
        }
    }
}
