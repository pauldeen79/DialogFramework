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
    public partial class RequiredValidationRule : DialogFramework.Domain.ValidationRule
    {
        public RequiredValidationRule() : base()
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.ValidationRuleBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.ValidationRules.RequiredValidationRuleBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.ValidationRules.RequiredValidationRuleBuilder(this);
        }
    }
#nullable restore
}