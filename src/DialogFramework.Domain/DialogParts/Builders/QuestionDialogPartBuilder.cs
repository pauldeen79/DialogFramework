namespace DialogFramework.Domain.DialogParts.Builders;

public partial class QuestionDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
