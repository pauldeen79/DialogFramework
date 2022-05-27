namespace DialogFramework.Domain.Extensions;

public static class DialogExtensions
{
    public static IDialogPart GetPartById(this IDialog dialog, string id)
    {
        if (id == dialog.AbortedPart.Id) return dialog.AbortedPart;
        if (id == dialog.CompletedPart.Id) return dialog.CompletedPart;
        if (id == dialog.ErrorPart.Id) return dialog.ErrorPart;
        var parts = dialog.Parts.Where(x => x.Id == id).ToArray();
        if (parts.Length == 1)
        {
            return parts[0];
        }
        if (parts.Length > 1)
        {
            throw new InvalidOperationException($"Dialog has multiple parts with id [{id}]");
        }
        throw new InvalidOperationException($"Dialog does not have a part with id [{id}]");
    }
}
