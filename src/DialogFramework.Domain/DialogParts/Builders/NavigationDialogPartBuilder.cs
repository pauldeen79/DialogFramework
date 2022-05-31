namespace DialogFramework.Domain.DialogParts.Builders;

public partial class NavigationDialogPartBuilder : IDialogPartBuilder
{
    IDialogPart IDialogPartBuilder.Build() => Build();
}
