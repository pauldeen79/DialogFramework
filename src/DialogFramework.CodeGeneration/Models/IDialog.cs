namespace DialogFramework.CodeGeneration.Models;

/// <summary>
/// Representation of a dialog with a user
/// </summary>
public interface IDialog
{
    [Required]
    string Id { get; }
    [Required]
    IReadOnlyCollection<IDialogPartResult> Results { get; }
}
