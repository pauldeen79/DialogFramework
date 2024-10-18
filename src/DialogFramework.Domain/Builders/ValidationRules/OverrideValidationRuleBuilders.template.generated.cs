﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.10
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace DialogFramework.Domain.Builders.ValidationRules
{
    public partial class ConditionalRequiredValidationRuleBuilder : DialogFramework.Domain.Builders.ValidationRuleBuilder<ConditionalRequiredValidationRuleBuilder, DialogFramework.Domain.ValidationRules.ConditionalRequiredValidationRule>
    {
        private ExpressionFramework.Domain.Builders.EvaluatableBuilder _condition;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public ExpressionFramework.Domain.Builders.EvaluatableBuilder Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Condition));
            }
        }

        public ConditionalRequiredValidationRuleBuilder(DialogFramework.Domain.ValidationRules.ConditionalRequiredValidationRule source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _condition = source.Condition.ToBuilder();
        }

        public ConditionalRequiredValidationRuleBuilder() : base()
        {
            _condition = new ExpressionFramework.Domain.Builders.Evaluatables.ConstantEvaluatableBuilder()!;
            SetDefaultValues();
        }

        public override DialogFramework.Domain.ValidationRules.ConditionalRequiredValidationRule BuildTyped()
        {
            return new DialogFramework.Domain.ValidationRules.ConditionalRequiredValidationRule(Condition.Build());
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.ValidationRules.ConditionalRequiredValidationRuleBuilder WithCondition(ExpressionFramework.Domain.Builders.EvaluatableBuilder condition)
        {
            if (condition is null) throw new System.ArgumentNullException(nameof(condition));
            Condition = condition;
            return this;
        }
    }
    public partial class RequiredValidationRuleBuilder : DialogFramework.Domain.Builders.ValidationRuleBuilder<RequiredValidationRuleBuilder, DialogFramework.Domain.ValidationRules.RequiredValidationRule>
    {
        public RequiredValidationRuleBuilder(DialogFramework.Domain.ValidationRules.RequiredValidationRule source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public RequiredValidationRuleBuilder() : base()
        {
            SetDefaultValues();
        }

        public override DialogFramework.Domain.ValidationRules.RequiredValidationRule BuildTyped()
        {
            return new DialogFramework.Domain.ValidationRules.RequiredValidationRule();
        }

        partial void SetDefaultValues();
    }
}
#nullable disable