namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogPartFixture
{
    public static IDialogPart CreateErrorThrowingDialogPart() => new ThrowingDialogPart();
    public static IDialogPartBuilder CreateErrorThrowingDialogPartBuilder() => new ThrowingDialogPartBuilder(CreateErrorThrowingDialogPart());
    public static AddPropertiesDialogPart CreateAddPropertiesDialogPart(Func<IAfterNavigateArguments, Result<IDialogPart>?> afterNavigateCallback, Func<IBeforeNavigateArguments, Result<IDialogPart>?> beforeNavigateCallback, DialogState state = DialogState.InProgress) => new AddPropertiesDialogPart(afterNavigateCallback, beforeNavigateCallback, state);
    public static AddPropertiesDialogPartBuilder CreateAddPropertiesDialogPartBuilder(Func<IAfterNavigateArguments, Result<IDialogPart>?> afterNavigateCallback, Func<IBeforeNavigateArguments, Result<IDialogPart>?> beforeNavigateCallback) => new AddPropertiesDialogPartBuilder(CreateAddPropertiesDialogPart(afterNavigateCallback, beforeNavigateCallback));

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

        public Result<IDialogPart>? AfterNavigate(IAfterNavigateArguments args)
            => default;

        public Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
            => throw new NotImplementedException();

        public IDialogPartBuilder CreateBuilder() => new ThrowingDialogPartBuilder(this);

        public DialogState GetState() => DialogState.InProgress;

        public bool SupportsReset() => false;
    }

    public sealed class AddPropertiesDialogPart : IDialogPart
    {
        public IDialogPartIdentifier Id { get; }
        private DialogState State { get; }
        private readonly Func<IAfterNavigateArguments, Result<IDialogPart>?> AfterNavigateCallback;
        private readonly Func<IBeforeNavigateArguments, Result<IDialogPart>?> BeforeNavigateCallback;

        public AddPropertiesDialogPart(Func<IAfterNavigateArguments, Result<IDialogPart>?> afterNavigateCallback, Func<IBeforeNavigateArguments, Result<IDialogPart>?> beforeNavigateCallback, DialogState state)
        {
            Id = new DialogPartIdentifier(string.Concat(nameof(AddPropertiesDialogPart), DateTime.Now.Ticks.ToString()));
            State = state;
            AfterNavigateCallback = afterNavigateCallback;
            BeforeNavigateCallback = beforeNavigateCallback;
        }

        public Result<IDialogPart>? AfterNavigate(IAfterNavigateArguments args) => AfterNavigateCallback(args);

        public Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args) => BeforeNavigateCallback(args);

        public IDialogPartBuilder CreateBuilder() => new AddPropertiesDialogPartBuilder(this);

        public DialogState GetState() => State;

        public bool SupportsReset() => false;
    }

    public class AddPropertiesDialogPartBuilder : IDialogPartBuilder
    {
        private readonly AddPropertiesDialogPart _addPropertiesDialogPart;

        public AddPropertiesDialogPartBuilder(AddPropertiesDialogPart addPropertiesDialogPart)
            => _addPropertiesDialogPart = addPropertiesDialogPart;

        public IDialogPart Build()
            => _addPropertiesDialogPart;
    }
}
