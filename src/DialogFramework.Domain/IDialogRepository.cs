﻿namespace DialogFramework.Domain;

public interface IDialogRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialog? GetDialog(IDialogIdentifier identifier);
}