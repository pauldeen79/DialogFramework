namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResultDefinition
{
    string Id { get; }
    string Title { get; }
    ResultValueType ValueType { get; }
}
