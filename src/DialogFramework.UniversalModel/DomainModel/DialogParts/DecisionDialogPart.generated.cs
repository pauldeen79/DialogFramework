﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 6.0.5
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
#nullable enable
    public partial record DecisionDialogPart : DialogFramework.Abstractions.DomainModel.DialogParts.IDecisionDialogPart
    {
        public string Id
        {
            get;
        }

        public DialogFramework.Abstractions.DomainModel.Domains.DialogState State
        {
            get;
        }

        public DecisionDialogPart(string id, DialogFramework.Abstractions.DomainModel.Domains.DialogState state)
        {
            this.Id = id;
            this.State = state;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }
    }
#nullable restore
}

