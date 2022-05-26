namespace DialogFramework.Domain.DomainModel.Builders;

public class DialogPartResultDefinitionValidatorBuilder
{
    private readonly IDialogPartResultDefinitionValidator _validator;

    public DialogPartResultDefinitionValidatorBuilder(IDialogPartResultDefinitionValidator validator)
        => _validator = validator;

    public IDialogPartResultDefinitionValidator Build()
        => _validator;
}
