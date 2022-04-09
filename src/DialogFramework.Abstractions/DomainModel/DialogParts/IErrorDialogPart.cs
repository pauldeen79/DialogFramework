using System;

namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IErrorDialogPart : IDialogPart
{
    string ErrorMessage { get; }
    IErrorDialogPart ForException(Exception ex);
}
