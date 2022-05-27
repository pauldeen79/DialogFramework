﻿namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record NavigationDialogPart
{
    public string GetNextPartId(IDialogContext context) => NavigateToId;
}