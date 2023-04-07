namespace DialogFramework.Domain.Contracts;

public interface IDialogRepository
{
    Result<DialogDefinition> Get(string id, string version);
}
