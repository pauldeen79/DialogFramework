namespace DialogFramework.Domain.DialogParts.Builders;

public partial class CompletedDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
