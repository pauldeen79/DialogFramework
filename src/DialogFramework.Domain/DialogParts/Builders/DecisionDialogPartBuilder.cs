namespace DialogFramework.Domain.DialogParts.Builders;

public partial class DecisionDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
