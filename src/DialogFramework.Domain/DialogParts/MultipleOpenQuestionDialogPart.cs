namespace DialogFramework.Domain.DialogParts;

public partial record MultipleOpenQuestionDialogPart : IValidatableDialogPart
{
    public Result Validate<T>(T value, Dialog dialog) => Validate(value, dialog, ValidationRules);
}

