namespace DialogFramework.Core.DomainModel;

public record DialogMetadata : IDialogMetadata
{
    public string Id { get; }
    public string FriendlyName { get; }
    public string Version { get; }
    public bool CanStart { get; }

    public DialogMetadata(string id,
                          string friendlyName,
                          string version,
                          bool canStart)
    {
        Id = id;
        FriendlyName = friendlyName;
        Version = version;
        CanStart = canStart;
    }
}
