namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPartSection
{
    [Required]
    string Id { get; }
    Evaluatable? Condition { get; }
    [Required]
    string Name { get; }
    [Required]
    IReadOnlyCollection<IDialogPart> Parts { get; }
}
