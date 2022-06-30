namespace DialogFramework.Application.Requests;

public record AbortRequest
{
    public IDialog Dialog { get; }

    public AbortRequest(IDialog dialog)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
    }
}
