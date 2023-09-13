namespace DialogFramework.Domain.Contracts;

public interface IDialogRepository
{
    Result<DialogDefinition> Get(string id, Version version);
}
