﻿namespace DialogFramework.CodeGeneration.Models;

/// <summary>
/// Representation of a dialog with a user
/// </summary>
public interface IDialog
{
    [Required] string Id { get; }
    [Required] string DefinitionId { get; }
    [Required] Version DefinitionVersion { get; }
    [Required] [ValidateObject] IReadOnlyCollection<IDialogPartResult> Results { get; }
    object? Context { get; }
}
