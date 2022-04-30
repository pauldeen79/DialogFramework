namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResult : IValidatableObject
{
    string DialogPartId { get; }
    string ResultId { get; }
    IDialogPartResultValue Value { get; }
}
