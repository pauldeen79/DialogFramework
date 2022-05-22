namespace DialogFramework.Core.Extensions;

internal static class DialogExtensions
{
    internal static IDialogPart GetFirstPart(this IDialog dialog,
                                             IDialogContext context,
                                             IDialogRepository dialogRepository)
    {
        var firstPart = dialog.Parts.FirstOrDefault();
        if (firstPart == null)
        {
            throw new InvalidOperationException("Could not determine next part. Dialog does not have any parts.");
        }

        return firstPart.ProcessDecisions(context, dialogRepository);
    }

    internal static IDialogPart GetNextPart(this IDialog dialog,
                                            IDialogContext context,
                                            IDialogPart currentPart,
                                            IDialogRepository dialogRepository,
                                            IEnumerable<IDialogPartResult> providedAnswers)
    {
        // first perform validation
        var error = currentPart.Validate(context, providedAnswers);
        if (error != null)
        {
            return error;
        }

        // if validation succeeds, then get the next part
        var parts = dialog.Parts.Select((part, index) => new { Index = index, Part = part }).ToArray();
        var currentPartWithIndex = parts.SingleOrDefault(p => p.Part.Id == currentPart.Id);
        var nextPartWithIndex = parts.Where(p => currentPartWithIndex != null && p.Index > currentPartWithIndex.Index).OrderBy(p => p.Index).FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return dialog.CompletedPart.ProcessDecisions(context, dialogRepository);
        }

        return nextPartWithIndex.Part.ProcessDecisions(context, dialogRepository);
    }

    internal static IDialogPart GetPartById(this IDialog dialog, string id)
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
