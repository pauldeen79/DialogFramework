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
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Conditions")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[ExpressionFramework.Abstractions.DomainModel.ICondition, ExpressionFramework.Abstractions, Version=0.2.19.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"NextPartId")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier"))
                    .WithName(@"IDecision"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Metadata")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogMetadata"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Parts")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogPart, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ErrorPart")
                            .WithTypeName(@"DialogFramework.Abstractions.DialogParts.IErrorDialogPart"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"AbortedPart")
                            .WithTypeName(@"DialogFramework.Abstractions.DialogParts.IAbortedDialogPart"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CompletedPart")
                            .WithTypeName(@"DialogFramework.Abstractions.DialogParts.ICompletedDialogPart"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"PartGroups")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogPartGroup, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"ReplaceAnswers")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"existingPartResults"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"newPartResults"))
                            .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanResetPartResultsByPartId")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"partId"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"ResetPartResultsByPartId")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"existingPartResults"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"partId"))
                            .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanNavigateTo")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"currentPartId"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"navigateToPartId"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"existingPartResults"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanStart")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetFirstPart")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPart"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetNextPart")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"providedResults"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPart"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"GetPartById")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"id"))
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPart"))
                    .WithName(@"IDialogDefinition"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentDialogIdentifier")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinitionIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentPartId")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentGroupId")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartGroupIdentifier")
                            .WithIsNullable(true),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CurrentState")
                            .WithTypeName(@"DialogFramework.Abstractions.DialogState"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Results")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogPartResult, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ValidationErrors")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogValidationResult, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Errors")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IError, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanStart")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Start")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"))
                            .WithTypeName(@"void"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanContinue")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"partResults"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Continue")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"partResults"),
                                new ParameterBuilder()
                                    .WithTypeName(@"ExpressionFramework.Abstractions.IConditionEvaluator")
                                    .WithName(@"conditionEvaluator"))
                            .WithTypeName(@"void"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanAbort")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Abort")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"))
                            .WithTypeName(@"void"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Error")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IError>")
                                    .WithName(@"errors"))
                            .WithTypeName(@"void"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanNavigateTo")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"navigateToPartId"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"NavigateTo")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier")
                                    .WithName(@"navigateToPartId"))
                            .WithTypeName(@"void"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"CanResetCurrentState")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"))
                            .WithTypeName(@"System.Boolean"),
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"ResetCurrentState")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"))
                            .WithTypeName(@"void"))
                    .WithName(@"IDialog"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.IIdentifier")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogIdentifier"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Version")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogDefinitionIdentifier"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.IDialogDefinitionIdentifier")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"FriendlyName")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"CanStart")
                            .WithTypeName(@"System.Boolean"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Version")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogMetadata"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartGroupIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Title")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Number")
                            .WithTypeName(@"System.Int32"))
                    .WithName(@"IDialogPartGroup"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.IIdentifier")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogPartGroupIdentifier"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.IIdentifier")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogPartIdentifier"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"DialogPartId")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ResultId")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartResultIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartResultValue"))
                    .WithName(@"IDialogPartResult"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Id")
                            .WithTypeName(@"DialogFramework.Abstractions.IDialogPartResultIdentifier"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Title")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ValueType")
                            .WithTypeName(@"DialogFramework.Abstractions.ResultValueType"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Validators")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogPartResultDefinitionValidator, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    .AddMethods(
                        new ClassMethodBuilder()
                            .WithVirtual(true)
                            .WithAbstract(true)
                            .WithName(@"Validate")
                            .AddParameters(
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialog")
                                    .WithName(@"dialog"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogDefinition")
                                    .WithName(@"dialogDefinition"),
                                new ParameterBuilder()
                                    .WithTypeName(@"DialogFramework.Abstractions.IDialogPart")
                                    .WithName(@"dialogPart"),
                                new ParameterBuilder()
                                    .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogPartResult>")
                                    .WithName(@"dialogPartResults"))
                            .WithTypeName(@"System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.IDialogValidationResult>"))
                    .WithName(@"IDialogPartResultDefinition"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddInterfaces(
                        @"DialogFramework.Abstractions.IIdentifier")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IDialogPartResultIdentifier"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Value")
                            .WithTypeName(@"System.Object")
                            .WithIsNullable(true),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ResultValueType")
                            .WithTypeName(@"DialogFramework.Abstractions.ResultValueType"))
                    .WithName(@"IDialogPartResultValue"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"ErrorMessage")
                            .WithTypeName(@"System.String"),
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"DialogPartResultIds")
                            .WithTypeName(@"System.Collections.Generic.IReadOnlyCollection`1[[DialogFramework.Abstractions.IDialogPartResultIdentifier, DialogFramework.Abstractions, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                    .WithName(@"IDialogValidationResult"),
                new InterfaceBuilder()
                    .WithNamespace(@"DialogFramework.Abstractions")
                    .AddProperties(
                        new ClassPropertyBuilder()
                            .WithHasSetter(false)
                            .WithName(@"Message")
                            .WithTypeName(@"System.String"))
                    .WithName(@"IError"),
            }.Select(x => x.Build()).ToArray();
        }
    }
#nullable restore
}
