namespace DialogFramework.Abstractions;

public interface IDialogService
{
    IDialogContext Start(IDialog dialog);
    IDialogContext Continue(IDialogContext context, IEnumerable<IProvidedAnswer> providedAnswers);
    IDialogContext Abort(IDialogContext context);
}
