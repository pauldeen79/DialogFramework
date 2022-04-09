namespace DialogFramework.Core.DomainModel.DialogParts;

public record ErrorDialogPart : IErrorDialogPart
{
    public ErrorDialogPart(string id, string errorMessage)
    {
        Id = id;
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
    public string Id { get; }

    public virtual IErrorDialogPart ForException(Exception ex)
    {
        return new ErrorDialogPart(Id, ex.Message);
    }
}
