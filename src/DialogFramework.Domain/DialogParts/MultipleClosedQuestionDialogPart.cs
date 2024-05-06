﻿namespace DialogFramework.Domain.DialogParts;

public partial class MultipleClosedQuestionDialogPart : IValidatableDialogPart
{
    public Result Validate<T>(T value, Dialog dialog) => Validate(value, dialog, ValidationRules);
}
