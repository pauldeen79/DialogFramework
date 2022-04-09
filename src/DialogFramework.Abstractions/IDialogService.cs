namespace DialogFramework.Abstractions;

public interface IDialogService
{
    IDialogContext Start(IDialog dialog);
    IDialogContext Continue(IDialogContext context, IEnumerable<KeyValuePair<string, object?>> answers);
    IDialogContext Abort(IDialogContext context);
}
