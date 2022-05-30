namespace DialogFramework.Abstractions;

public interface IDialogPartGroup
{
    IDialogPartGroupIdentifier Id { get; }
    string Title { get; }
    int Number { get; }
}
