namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IRedirectDialogPart : IDialogPart
{
    IDialog RedirectDialog { get; }
}
