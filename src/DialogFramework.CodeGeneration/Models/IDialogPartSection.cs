namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPartSection
{
    [Required] string Id { get; }
    [ValidateObject] Evaluatable? Condition { get; }
    [Required] string Name { get; }
    [Required][ValidateObject] IReadOnlyCollection<IDialogPart> Parts { get; }
}
