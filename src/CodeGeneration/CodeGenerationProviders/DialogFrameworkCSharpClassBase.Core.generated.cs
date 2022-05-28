﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 6.0.5
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using ModelFramework.Objects.Builders;
using ModelFramework.Objects.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration.CodeGenerationProviders
{
#nullable enable
    public partial class DialogFrameworkCSharpClassBase
    {
        protected static ITypeBase[] GetCoreModels()
        {
            return new[]
            {
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentDialogIdentifier")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentPart")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentGroup")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPartGroup")
                            .WithIsNullable(true),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentState")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.Domains.DialogState"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Results")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.DomainModel.IDialogPartResult, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanStart")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Start")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart")
                                    .WithName(@"firstPart"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"AddDialogPartResults")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IDialogPartResult>")
                                    .WithName(@"dialogPartResults"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanContinue")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Continue")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart")
                                    .WithName(@"nextPart"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanAbort")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Abort")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Error")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Exception")
                                    .WithIsNullable(true)
                                    .WithName(@"exception"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanNavigateTo")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart")
                                    .WithName(@"navigateToPart"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"NavigateTo")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart")
                                    .WithName(@"navigateToPart"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanResetCurrentState")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"ResetCurrentState")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogContext"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetDialogPartResultsByPartIdentifier")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"System.String")
                                    .WithName(@"dialogPartIdentifier"))
                            .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IDialogPartResult>"))
                    .WithName(@"IDialogContext"),
            }.Select(x => x.Build()).ToArray();
        }
    }
#nullable restore
}
