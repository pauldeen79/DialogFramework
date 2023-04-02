namespace DialogFramework.CodeGeneration.Models.DialogParts;

public interface ILabelDialogPart : IDialogPart
{
    [Required]
    string Id { get; }
    Evaluatable? Condition { get; }
    [Required]
    string Caption { get; }
}
