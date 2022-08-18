namespace DialogFramework.Domain;

public partial record DialogPartResultAnswerDefinition
{
    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IDialogPart part,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
    {
        foreach (var validator in Validators)
        {
            foreach (var validationError in validator.Validate(dialog, definition, part, this, answers))
            {
                yield return validationError;
            }
        }
    }
}
