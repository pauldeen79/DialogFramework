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

namespace DialogFramework.Core.DomainModel.DialogParts
{
#nullable enable
    public partial record QuestionDialogPart : DialogFramework.Abstractions.DomainModel.DialogParts.IQuestionDialogPart
    {
        public string Title
        {
            get;
        }

        public CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IDialogPartResultDefinition> Results
        {
            get;
        }

        public CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IQuestionDialogPartValidator> Validators
        {
            get;
        }

        public CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IDialogValidationResult> ValidationErrors
        {
            get;
        }

        public DialogFramework.Abstractions.DomainModel.IDialogPartGroup Group
        {
            get;
        }

        public string Heading
        {
            get;
        }

        public string Id
        {
            get;
        }

        public QuestionDialogPart(string title, System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IDialogPartResultDefinition> results, System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IQuestionDialogPartValidator> validators, System.Collections.Generic.IEnumerable<DialogFramework.Abstractions.DomainModel.IDialogValidationResult> validationErrors, DialogFramework.Abstractions.DomainModel.IDialogPartGroup group, string heading, string id)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            if (validators == null) throw new System.ArgumentNullException("validators");
            if (validationErrors == null) throw new System.ArgumentNullException("validationErrors");
            if (group == null) throw new System.ArgumentNullException("group");
            this.Title = title;
            this.Results = new CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IDialogPartResultDefinition>(results);
            this.Validators = new CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IQuestionDialogPartValidator>(validators);
            this.ValidationErrors = new CrossCutting.Common.ValueCollection<DialogFramework.Abstractions.DomainModel.IDialogValidationResult>(validationErrors);
            this.Group = group;
            this.Heading = heading;
            this.Id = id;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }
    }
#nullable restore
}
