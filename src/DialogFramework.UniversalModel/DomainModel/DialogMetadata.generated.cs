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

namespace DialogFramework.UniversalModel.DomainModel
{
#nullable enable
    public partial record DialogMetadata : DialogFramework.Abstractions.DomainModel.IDialogMetadata
    {
        public string FriendlyName
        {
            get;
        }

        public bool CanStart
        {
            get;
        }

        public string Id
        {
            get;
        }

        public string Version
        {
            get;
        }

        public DialogMetadata(string friendlyName, bool canStart, string id, string version)
        {
            this.FriendlyName = friendlyName;
            this.CanStart = canStart;
            this.Id = id;
            this.Version = version;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }
    }
#nullable restore
}

