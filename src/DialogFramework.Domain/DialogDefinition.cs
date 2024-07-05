namespace DialogFramework.Domain;

public partial class DialogDefinition : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateSectionIds = Sections.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToArray();
        if (duplicateSectionIds.Any())
        {
            yield return new ValidationResult($"Duplicate section ids: {string.Join(", ", duplicateSectionIds.Select(x => x.Key))}", [nameof(Sections)]);
        }

        var duplicatePartIds = Sections.SelectMany(x => x.Parts).GroupBy(x => x.Id).Where(x => x.Count() > 1).ToArray();
        if (duplicatePartIds.Any())
        {
            yield return new ValidationResult($"Duplicate part ids: {string.Join(", ", duplicatePartIds.Select(x => x.Key))}", [nameof(Sections)]);
        }
    }

    public IEnumerable<DialogPart> GetAllParts() => Sections.SelectMany(x => x.Parts);
}
