namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogMetadata
{
    string Id { get; }
    string FriendlyName { get; }
    string Version { get; }
    bool CanStart { get; }
}
