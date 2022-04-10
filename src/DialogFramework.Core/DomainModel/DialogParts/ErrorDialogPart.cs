namespace DialogFramework.Core.DomainModel.DialogParts;

public record ErrorDialogPart : IErrorDialogPart
{
    public ErrorDialogPart(string id, string errorMessage, Exception? exception)
    {
        Id = id;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public string ErrorMessage { get; }
    public string Id { get; }
    public Exception? Exception { get; }
    public DialogState State => DialogState.ErrorOccured;

    public virtual IErrorDialogPart ForException(Exception ex)
        => new ErrorDialogPart(Id, ErrorMessage, ex);
}
