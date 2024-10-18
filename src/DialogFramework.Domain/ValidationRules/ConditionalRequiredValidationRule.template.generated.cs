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

namespace DialogFramework.Domain.ValidationRules
{
#nullable enable
    public partial class ConditionalRequiredValidationRule : DialogFramework.Domain.ValidationRule
    {
        private ExpressionFramework.Domain.Evaluatable _condition;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public ExpressionFramework.Domain.Evaluatable Condition
        {
            get
            {
                return _condition;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<ExpressionFramework.Domain.Evaluatable>.Default.Equals(_condition, value);
                _condition = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Condition));
            }
        }

        public ConditionalRequiredValidationRule(ExpressionFramework.Domain.Evaluatable condition) : base()
        {
            this._condition = condition;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.ValidationRuleBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.ValidationRules.ConditionalRequiredValidationRuleBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.ValidationRules.ConditionalRequiredValidationRuleBuilder(this);
        }
    }
#nullable restore
}
