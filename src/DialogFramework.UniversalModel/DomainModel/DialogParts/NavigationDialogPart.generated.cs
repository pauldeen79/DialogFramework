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
    public partial record NavigationDialogPart : DialogFramework.Abstractions.DomainModel.DialogParts.INavigationDialogPart
    {
        public string Id
        {
            get;
        }

        public DialogFramework.Abstractions.DomainModel.Domains.DialogState State
        {
            get;
        }

        public string NavigateToId
        {
            get;
        }

        public NavigationDialogPart(string id, DialogFramework.Abstractions.DomainModel.Domains.DialogState state, string navigateToId)
        {
            if (navigateToId == null) throw new System.ArgumentNullException("navigateToId");
            this.Id = id;
            this.State = state;
            this.NavigateToId = navigateToId;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }
    }
#nullable restore
}

