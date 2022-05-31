namespace DialogFramework.Domain.DialogParts.Builders;

public partial class MessageDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
