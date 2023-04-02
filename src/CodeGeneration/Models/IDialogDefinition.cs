namespace DialogFramework.CodeGeneration.Models;

/// <summary>
/// Definition of a dialog that can be performed
/// </summary>
public interface IDialogDefinition
{
    [Required]
    string Id { get; }
    [Required]
    IReadOnlyCollection<IDialogPartSection> Sections { get; }
}
