namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IRedirectDialogPart : IDialogPart
{
    IDialogMetadata RedirectDialogMetadata { get; }
}
