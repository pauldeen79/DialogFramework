namespace DialogFramework.Abstractions;

public interface IDialogPart
{
    IDialogPartIdentifier Id { get; }
    DialogState GetState();
    IDialogPartBuilder CreateBuilder();
    bool SupportsReset();
    void BeforeNavigate(IBeforeNavigateArguments args);
    void AfterNavigate(IAfterNavigateArguments args);
}
