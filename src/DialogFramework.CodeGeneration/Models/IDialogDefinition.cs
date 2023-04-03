﻿namespace DialogFramework.CodeGeneration.Models;

/// <summary>
/// Definition of a dialog that can be performed
/// </summary>
public interface IDialogDefinition
{
    [Required]
    string Id { get; }
    [Required]
    string Name { get; }
    [Required]
    string Version { get; }
    [Required]
    IReadOnlyCollection<IDialogPartSection> Sections { get; }
}
