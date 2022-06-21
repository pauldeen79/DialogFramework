namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogPartFixture
{
    public static IDialogPart CreateErrorThrowingDialogPart() => new ThrowingDialogPart();
    public static IDialogPartBuilder CreateErrorThrowingDialogPartBuilder() => new ThrowingDialogPartBuilder(CreateErrorThrowingDialogPart());

    [ExcludeFromCodeCoverage]
    private sealed class ThrowingDialogPartBuilder : IDialogPartBuilder
    {
        private readonly IDialogPart _part;
        public IDialogPart Build() => _part;
        public ThrowingDialogPartBuilder(IDialogPart part) => _part = part;
    }

    [ExcludeFromCodeCoverage]
    private sealed class ThrowingDialogPart : INavigationDialogPart
    {
        public IDialogPartIdentifier Id => new DialogPartIdentifier(nameof(ThrowingDialogPart));

        public IDialogPartBuilder CreateBuilder() => new ThrowingDialogPartBuilder(this);

        public IDialogPartIdentifier GetNextPartId(IDialog dialog)
        {
            throw new NotImplementedException();
        }

        public DialogState GetState() => DialogState.InProgress;

        public bool SupportsReset() => false;
    }
}
