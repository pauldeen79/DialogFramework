namespace DialogFramework.Domain;

public partial record DialogPartResultDefinition
{
    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IDialogPart dialogPart,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
    {
        foreach (var validator in Validators)
        {
            foreach (var validationError in validator.Validate(context, dialog, dialogPart, this, dialogPartResults))
            {
                yield return validationError;
            }
        }
    }
}
