namespace DialogFramework.Core.DomainModel;

public record DialogIdentifier : IDialogIdentifier
{
    public string Id { get; }
    public string Version { get; }

    public DialogIdentifier(string id, string version)
    {
        Id = id;
        Version = version;
    }
}
