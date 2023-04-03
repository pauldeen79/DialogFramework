namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPartResult
{
    [Required]
    string DefinitionId { get; }
    [Required]
    string PartId { get; }
}
