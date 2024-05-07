﻿namespace DialogFramework.Domain.DialogParts;

public partial record SingleClosedQuestionDialogPart : IValidatableDialogPart
{
    public Result Validate<T>(T value, Dialog dialog) => Validate(value, dialog, ValidationRules);
}
