namespace DialogFramework.Domain.DialogParts.Builders;

public partial class RedirectDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
