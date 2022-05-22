namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogMetadata : IDialogIdentifier
{
    string FriendlyName { get; }
    bool CanStart { get; }
}
