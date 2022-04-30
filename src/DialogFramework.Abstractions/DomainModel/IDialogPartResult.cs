namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResult
{
    string DialogPartId { get; }
    string ResultId { get; }
    IDialogPartResultValue Value { get; }
}
