namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartGroup
{
    string Id { get; }
    string Title { get; }
    int GroupNumber { get; }
}
