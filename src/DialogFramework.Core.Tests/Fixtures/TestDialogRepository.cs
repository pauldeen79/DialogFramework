﻿namespace DialogFramework.Core.Tests.Fixtures;

internal class TestDialogRepository : IDialogRepository
{
    private static readonly IDialog[] _dialogs = new IDialog[]
    {
        new TestFlowDialog(),
        new SimpleFormFlowDialog()
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogs.Select(x => x.Metadata);

    public IDialog GetDialog(string id, string version)
        => _dialogs.Single(x => x.Metadata.Id == id && x.Metadata.Version == version);
}