namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogPartFixture
{
    public static IDialogPart CreateErrorThrowingDialogPart() => new ThrowingDialogPart();
    public static IDialogPartBuilder CreateErrorThrowingDialogPartBuilder() => new ThrowingDialogPartBuilder(CreateErrorThrowingDialogPart());
    public static AddPropertiesDialogPart CreateAddPropertiesDialogPart(Action<IAfterNavigateArguments> afterNavigateCallback, Action<IBeforeNavigateArguments> beforeNavigateCallback, DialogState state = DialogState.InProgress) => new AddPropertiesDialogPart(afterNavigateCallback, beforeNavigateCallback, state);
    public static AddPropertiesDialogPartBuilder CreateAddPropertiesDialogPartBuilder(Action<IAfterNavigateArguments> afterNavigateCallback, Action<IBeforeNavigateArguments> beforeNavigateCallback) => new AddPropertiesDialogPartBuilder(CreateAddPropertiesDialogPart(afterNavigateCallback, beforeNavigateCallback));

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

        public void AfterNavigate(IAfterNavigateArguments args)
        {
            // Method intentionally left empty.
        }

        public void BeforeNavigate(IBeforeNavigateArguments args)
        {
            // Method intentionally left empty.
        }

        public IDialogPartBuilder CreateBuilder() => new ThrowingDialogPartBuilder(this);

        public IDialogPartIdentifier GetNextPartId(IDialog dialog)
        {
            throw new NotImplementedException();
        }

        public DialogState GetState() => DialogState.InProgress;

        public bool SupportsReset() => false;
    }

    public sealed class AddPropertiesDialogPart : IDialogPart
    {
        public IDialogPartIdentifier Id { get; }
        private DialogState State { get; }
        private readonly Action<IAfterNavigateArguments> AfterNavigateCallback;
        private readonly Action<IBeforeNavigateArguments> BeforeNavigateCallback;

        public AddPropertiesDialogPart(Action<IAfterNavigateArguments> afterNavigateCallback, Action<IBeforeNavigateArguments> beforeNavigateCallback, DialogState state)
        {
            Id = new DialogPartIdentifier(string.Concat(nameof(AddPropertiesDialogPart), DateTime.Now.Ticks.ToString()));
            State = state;
            AfterNavigateCallback = afterNavigateCallback;
            BeforeNavigateCallback = beforeNavigateCallback;
        }

        public void AfterNavigate(IAfterNavigateArguments args) => AfterNavigateCallback(args);

        public void BeforeNavigate(IBeforeNavigateArguments args) => BeforeNavigateCallback(args);

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
