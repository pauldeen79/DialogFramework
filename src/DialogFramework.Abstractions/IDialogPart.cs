namespace DialogFramework.Abstractions;

public interface IDialogPart
{
    IDialogPartIdentifier Id { get; }
    DialogState GetState();
    IDialogPartBuilder CreateBuilder();
    bool SupportsReset();
    Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args);
    Result<IDialogPart>? AfterNavigate(IAfterNavigateArguments args);
}
