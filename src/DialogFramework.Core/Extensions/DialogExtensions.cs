namespace DialogFramework.Core.Extensions;

internal static class DialogExtensions
{
    internal static IDialogPart GetNextPart(this IDialog dialog,
                                            IDialogContext context,
                                            IDialogPart? currentPart,
                                            IEnumerable<IProvidedAnswer> providedAnswers)
    {
        if (currentPart == null)
        {
            // get the first part
            var firstPart = dialog.Parts.FirstOrDefault();
            if (firstPart == null)
            {
                throw new InvalidOperationException("Could not determine next part. Dialog does not have any parts.");
            }

            return firstPart.ProcessDecisions(context);
        }

        // first perform validation
        var error = currentPart.Validate(providedAnswers);
        if (error != null)
        {
            return error;
        }

        // if validation succeeds, then get the next part
        var parts = dialog.Parts.Select((part, index) => new { Index = index, Part = part }).ToArray();
        var currentPartWithIndex = parts.SingleOrDefault(p => p.Part.Id == currentPart!.Id);
        var nextPartWithIndex = parts.Where(p => p.Index > currentPartWithIndex.Index).OrderBy(p => p.Index).FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return dialog.CompletedPart.ProcessDecisions(context);
        }

        return nextPartWithIndex.Part.ProcessDecisions(context);
    }
}
