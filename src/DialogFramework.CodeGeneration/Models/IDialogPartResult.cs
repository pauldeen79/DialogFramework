namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPartResult
{
    [Required]
    string DialogPartId { get; }
    [Required]
    string ResultId { get; }
    object? Value { get; }
}
