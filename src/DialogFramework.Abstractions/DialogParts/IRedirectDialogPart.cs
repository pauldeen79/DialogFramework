namespace DialogFramework.Abstractions.DialogParts;

public interface IRedirectDialogPart : IDialogPart
{
    IDialogMetadata RedirectDialogMetadata { get; }
}
