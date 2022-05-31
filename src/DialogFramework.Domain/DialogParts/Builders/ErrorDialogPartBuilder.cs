namespace DialogFramework.Domain.DialogParts.Builders;

public partial class ErrorDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
