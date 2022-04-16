namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogPartResult : IValidatableObject
{
    IDialogPart DialogPart { get; }
    IDialogPartResultDefinition Result { get; }
    IDialogPartResultValue Value { get; }
}
