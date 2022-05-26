namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record ErrorDialogPart
{
    public IErrorDialogPart ForException(Exception ex)
        => new ErrorDialogPart(ErrorMessage, ex, Id);
}
