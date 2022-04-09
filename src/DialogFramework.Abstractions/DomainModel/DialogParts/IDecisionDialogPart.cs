namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IDecisionDialogPart : IDialogPart, IDataErrorInfo
{
    IDialogPart? NextPart { get; }
}
