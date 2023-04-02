namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPartSection
{
    [Required]
    string Id { get; }
    [Required]
    IReadOnlyCollection<IDialogPart> Parts { get; }
}
