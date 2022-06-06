namespace DialogFramework.Abstractions;

public interface IDialogMetadata : IDialogDefinitionIdentifier
{
    string FriendlyName { get; }
    bool CanStart { get; }
}
