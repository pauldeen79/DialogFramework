namespace DialogFramework.CodeGeneration.Models;

public interface IDialogPart
{
    [Required] string Id { get; }
    [ValidateObject] Evaluatable? Condition { get; }
    [Required] string Title { get; }
}
