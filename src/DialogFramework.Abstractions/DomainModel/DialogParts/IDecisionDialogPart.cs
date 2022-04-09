﻿namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    IDialogPart GetNextPart(IDialogContext context, IEnumerable<KeyValuePair<string, object?>> answerValues);
}
