﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.8
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
namespace DialogFramework.Domain.DialogParts
{
    public partial class LabelDialogPart : DialogFramework.Domain.DialogPart
    {
        public LabelDialogPart(string id, ExpressionFramework.Domain.Evaluatable? condition, string title) : base(id, condition, title)
        {
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.DialogPartBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.DialogParts.LabelDialogPartBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.DialogParts.LabelDialogPartBuilder(this);
        }
    }
    public partial class MultipleClosedQuestionDialogPart : DialogFramework.Domain.DialogPart
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption> _options;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption> Options
        {
            get
            {
                return _options;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>>.Default.Equals(_options, value);
                _options = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Options));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>>.Default.Equals(_validationRules, value);
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public MultipleClosedQuestionDialogPart(System.Collections.Generic.IEnumerable<DialogFramework.Domain.ClosedQuestionOption> options, string id, ExpressionFramework.Domain.Evaluatable? condition, string title, System.Collections.Generic.IEnumerable<DialogFramework.Domain.ValidationRule> validationRules) : base(id, condition, title)
        {
            this._options = options is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>(options);
            this._validationRules = validationRules is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(validationRules);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.DialogPartBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder(this);
        }
    }
    public partial class MultipleOpenQuestionDialogPart : DialogFramework.Domain.DialogPart
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>>.Default.Equals(_validationRules, value);
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public MultipleOpenQuestionDialogPart(string id, ExpressionFramework.Domain.Evaluatable? condition, string title, System.Collections.Generic.IEnumerable<DialogFramework.Domain.ValidationRule> validationRules) : base(id, condition, title)
        {
            this._validationRules = validationRules is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(validationRules);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.DialogPartBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleOpenQuestionDialogPartBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.DialogParts.MultipleOpenQuestionDialogPartBuilder(this);
        }
    }
    public partial class SingleClosedQuestionDialogPart : DialogFramework.Domain.DialogPart
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption> _options;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption> Options
        {
            get
            {
                return _options;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>>.Default.Equals(_options, value);
                _options = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Options));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>>.Default.Equals(_validationRules, value);
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public SingleClosedQuestionDialogPart(System.Collections.Generic.IEnumerable<DialogFramework.Domain.ClosedQuestionOption> options, string id, ExpressionFramework.Domain.Evaluatable? condition, string title, System.Collections.Generic.IEnumerable<DialogFramework.Domain.ValidationRule> validationRules) : base(id, condition, title)
        {
            this._options = options is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>(options);
            this._validationRules = validationRules is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(validationRules);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.DialogPartBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder(this);
        }
    }
    public partial class SingleOpenQuestionDialogPart : DialogFramework.Domain.DialogPart
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            private set
            {
                bool hasChanged = !EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>>.Default.Equals(_validationRules, value);
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public SingleOpenQuestionDialogPart(string id, ExpressionFramework.Domain.Evaluatable? condition, string title, System.Collections.Generic.IEnumerable<DialogFramework.Domain.ValidationRule> validationRules) : base(id, condition, title)
        {
            this._validationRules = validationRules is null ? null! : new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(validationRules);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override DialogFramework.Domain.Builders.DialogPartBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleOpenQuestionDialogPartBuilder ToTypedBuilder()
        {
            return new DialogFramework.Domain.Builders.DialogParts.SingleOpenQuestionDialogPartBuilder(this);
        }
    }
}
#nullable disable
