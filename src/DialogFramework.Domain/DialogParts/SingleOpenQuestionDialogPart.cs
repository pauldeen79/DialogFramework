namespace DialogFramework.Domain.DialogParts;

public partial record SingleOpenQuestionDialogPart : IValidatableDialogPart
{
    public Result Validate<T>(T value, Dialog dialog) => Validate(value, dialog, ValidationRules);
}

