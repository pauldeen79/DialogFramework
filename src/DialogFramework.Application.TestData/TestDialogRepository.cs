﻿namespace DialogFramework.Application.TestData;

public class TestDialogRepository : IDialogRepository
{
    private static readonly IDialog[] _dialogs = new[]
    {
        DialogFixture.CreateBuilder().Build(),
        DialogFixture.CreateHowDoYouFeelBuilder().Build(),
        TestFlowDialog.Create(),
        SimpleFormFlowDialog.Create(),
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogs.Select(x => x.Metadata);

    public IDialog? GetDialog(IDialogIdentifier identifier)
        => _dialogs.SingleOrDefault(x => Equals(x.Metadata.Id, identifier.Id) && Equals(x.Metadata.Version, identifier.Version));
}