﻿namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    string GetNextPartId(IDialogContext context);
}
