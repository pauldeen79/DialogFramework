namespace DialogFramework.Domain.Builders;

public class DialogPartResultAnswerDefinitionValidatorBuilder
{
    private readonly IDialogPartResultAnswerDefinitionValidator _validator;

    public DialogPartResultAnswerDefinitionValidatorBuilder(IDialogPartResultAnswerDefinitionValidator validator)
        => _validator = validator;

    public IDialogPartResultAnswerDefinitionValidator Build()
        => _validator;
}
