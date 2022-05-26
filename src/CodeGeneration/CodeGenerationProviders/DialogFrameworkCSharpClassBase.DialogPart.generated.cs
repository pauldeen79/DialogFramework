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
        protected static ITypeBase[] GetDialogPartModels()
        {
            return new[]
            {
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Message")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IAbortedDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.DialogParts.IGroupedDialogPart",
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Message")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Group")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPartGroup"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Heading")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .WithName(@"ICompletedDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetNextPartId")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogContext")
                                    .WithName(@"context"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"evaluator"))
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDecisionDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ErrorMessage")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Exception")
                            .WithTypeName(@"System.Exception")
                            .WithIsNullable(true),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"ForException")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Exception")
                                    .WithName(@"ex"))
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.DialogParts.IErrorDialogPart"))
                    .WithName(@"IErrorDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.DialogParts.IGroupedDialogPart",
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Message")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Group")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPartGroup"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Heading")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IMessageDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetNextPartId")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogContext")
                                    .WithName(@"context"))
                            .WithTypeName(@"System.String"))
                    .WithName(@"INavigationDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.DialogParts.IGroupedDialogPart",
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Title")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Results")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.DomainModel.IDialogPartResultDefinition, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Validators")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.DomainModel.IQuestionDialogPartValidator, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ValidationErrors")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.DomainModel.IDialogValidationResult, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Group")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPartGroup"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Heading")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Validate")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogContext")
                                    .WithName(@"context"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IDialogPartResult>")
                                    .WithName(@"dialogPartResults"))
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogPart"))
                    .WithName(@"IQuestionDialogPart"),
                new ClassBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions.DomainModel.DialogParts")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.DomainModel.IDialogPart")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"RedirectDialogMetadata")
                            .WithTypeName(@"DialogFramework.Abstractions.DomainModel.IDialogMetadata"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IRedirectDialogPart"),
            }.Select(x => x.Build()).ToArray();
        }
    }
#nullable restore
}
