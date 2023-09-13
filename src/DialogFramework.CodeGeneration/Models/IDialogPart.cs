namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPart
{
    [Required]
    string Id { get; }
    Evaluatable? Condition { get; }
    [Required]
    string Title { get; }
}
