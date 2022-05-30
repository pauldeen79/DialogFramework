namespace DialogFramework.Abstractions;

public interface IDialogMetadata : IDialogIdentifier
{
    string FriendlyName { get; }
    bool CanStart { get; }
}
