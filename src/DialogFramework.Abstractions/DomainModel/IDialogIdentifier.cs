namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogIdentifier
{
    string Id { get; }
    string Version { get; }
}
