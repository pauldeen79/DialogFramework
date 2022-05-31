namespace DialogFramework.Domain.DialogParts.Builders;

public partial class AbortedDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
