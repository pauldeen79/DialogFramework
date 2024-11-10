﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.10
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace DialogFramework.Domain.Builders.DialogParts
{
    public partial class LabelDialogPartBuilder : DialogFramework.Domain.Builders.DialogPartBuilder<LabelDialogPartBuilder, DialogFramework.Domain.DialogParts.LabelDialogPart>
    {
        public LabelDialogPartBuilder(DialogFramework.Domain.DialogParts.LabelDialogPart source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public LabelDialogPartBuilder() : base()
        {
            SetDefaultValues();
        }

        public override DialogFramework.Domain.DialogParts.LabelDialogPart BuildTyped()
        {
            return new DialogFramework.Domain.DialogParts.LabelDialogPart(Id, Condition?.Build(), Title);
        }

        partial void SetDefaultValues();
    }
    public partial class MultipleClosedQuestionDialogPartBuilder : DialogFramework.Domain.Builders.DialogPartBuilder<MultipleClosedQuestionDialogPartBuilder, DialogFramework.Domain.DialogParts.MultipleClosedQuestionDialogPart>
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> _options;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Options));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            set
            {
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public MultipleClosedQuestionDialogPartBuilder(DialogFramework.Domain.DialogParts.MultipleClosedQuestionDialogPart source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _options = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder>();
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            if (source.Options is not null) foreach (var item in source.Options.Select(x => x.ToBuilder())) _options.Add(item);
            if (source.ValidationRules is not null) foreach (var item in source.ValidationRules.Select(x => x.ToBuilder())) _validationRules.Add(item);
        }

        public MultipleClosedQuestionDialogPartBuilder() : base()
        {
            _options = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder>();
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            SetDefaultValues();
        }

        public override DialogFramework.Domain.DialogParts.MultipleClosedQuestionDialogPart BuildTyped()
        {
            return new DialogFramework.Domain.DialogParts.MultipleClosedQuestionDialogPart(new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>(Options.Select(x => x.Build()!)), Id, Condition?.Build(), Title, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(ValidationRules.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder AddOptions(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> options)
        {
            if (options is null) throw new System.ArgumentNullException(nameof(options));
            return AddOptions(options.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder AddOptions(params DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder[] options)
        {
            if (options is null) throw new System.ArgumentNullException(nameof(options));
            foreach (var item in options) Options.Add(item);
            return this;
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder AddValidationRules(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ValidationRuleBuilder> validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            return AddValidationRules(validationRules.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleClosedQuestionDialogPartBuilder AddValidationRules(params DialogFramework.Domain.Builders.ValidationRuleBuilder[] validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            foreach (var item in validationRules) ValidationRules.Add(item);
            return this;
        }
    }
    public partial class MultipleOpenQuestionDialogPartBuilder : DialogFramework.Domain.Builders.DialogPartBuilder<MultipleOpenQuestionDialogPartBuilder, DialogFramework.Domain.DialogParts.MultipleOpenQuestionDialogPart>
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            set
            {
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public MultipleOpenQuestionDialogPartBuilder(DialogFramework.Domain.DialogParts.MultipleOpenQuestionDialogPart source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            if (source.ValidationRules is not null) foreach (var item in source.ValidationRules.Select(x => x.ToBuilder())) _validationRules.Add(item);
        }

        public MultipleOpenQuestionDialogPartBuilder() : base()
        {
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            SetDefaultValues();
        }

        public override DialogFramework.Domain.DialogParts.MultipleOpenQuestionDialogPart BuildTyped()
        {
            return new DialogFramework.Domain.DialogParts.MultipleOpenQuestionDialogPart(Id, Condition?.Build(), Title, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(ValidationRules.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogParts.MultipleOpenQuestionDialogPartBuilder AddValidationRules(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ValidationRuleBuilder> validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            return AddValidationRules(validationRules.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.MultipleOpenQuestionDialogPartBuilder AddValidationRules(params DialogFramework.Domain.Builders.ValidationRuleBuilder[] validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            foreach (var item in validationRules) ValidationRules.Add(item);
            return this;
        }
    }
    public partial class SingleClosedQuestionDialogPartBuilder : DialogFramework.Domain.Builders.DialogPartBuilder<SingleClosedQuestionDialogPartBuilder, DialogFramework.Domain.DialogParts.SingleClosedQuestionDialogPart>
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> _options;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Options));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            set
            {
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public SingleClosedQuestionDialogPartBuilder(DialogFramework.Domain.DialogParts.SingleClosedQuestionDialogPart source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _options = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder>();
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            if (source.Options is not null) foreach (var item in source.Options.Select(x => x.ToBuilder())) _options.Add(item);
            if (source.ValidationRules is not null) foreach (var item in source.ValidationRules.Select(x => x.ToBuilder())) _validationRules.Add(item);
        }

        public SingleClosedQuestionDialogPartBuilder() : base()
        {
            _options = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder>();
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            SetDefaultValues();
        }

        public override DialogFramework.Domain.DialogParts.SingleClosedQuestionDialogPart BuildTyped()
        {
            return new DialogFramework.Domain.DialogParts.SingleClosedQuestionDialogPart(new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ClosedQuestionOption>(Options.Select(x => x.Build()!)), Id, Condition?.Build(), Title, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(ValidationRules.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder AddOptions(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder> options)
        {
            if (options is null) throw new System.ArgumentNullException(nameof(options));
            return AddOptions(options.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder AddOptions(params DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder[] options)
        {
            if (options is null) throw new System.ArgumentNullException(nameof(options));
            foreach (var item in options) Options.Add(item);
            return this;
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder AddValidationRules(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ValidationRuleBuilder> validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            return AddValidationRules(validationRules.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleClosedQuestionDialogPartBuilder AddValidationRules(params DialogFramework.Domain.Builders.ValidationRuleBuilder[] validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            foreach (var item in validationRules) ValidationRules.Add(item);
            return this;
        }
    }
    public partial class SingleOpenQuestionDialogPartBuilder : DialogFramework.Domain.Builders.DialogPartBuilder<SingleOpenQuestionDialogPartBuilder, DialogFramework.Domain.DialogParts.SingleOpenQuestionDialogPart>
    {
        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> _validationRules;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder> ValidationRules
        {
            get
            {
                return _validationRules;
            }
            set
            {
                _validationRules = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(ValidationRules));
            }
        }

        public SingleOpenQuestionDialogPartBuilder(DialogFramework.Domain.DialogParts.SingleOpenQuestionDialogPart source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            if (source.ValidationRules is not null) foreach (var item in source.ValidationRules.Select(x => x.ToBuilder())) _validationRules.Add(item);
        }

        public SingleOpenQuestionDialogPartBuilder() : base()
        {
            _validationRules = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.ValidationRuleBuilder>();
            SetDefaultValues();
        }

        public override DialogFramework.Domain.DialogParts.SingleOpenQuestionDialogPart BuildTyped()
        {
            return new DialogFramework.Domain.DialogParts.SingleOpenQuestionDialogPart(Id, Condition?.Build(), Title, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.ValidationRule>(ValidationRules.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogParts.SingleOpenQuestionDialogPartBuilder AddValidationRules(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.ValidationRuleBuilder> validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            return AddValidationRules(validationRules.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogParts.SingleOpenQuestionDialogPartBuilder AddValidationRules(params DialogFramework.Domain.Builders.ValidationRuleBuilder[] validationRules)
        {
            if (validationRules is null) throw new System.ArgumentNullException(nameof(validationRules));
            foreach (var item in validationRules) ValidationRules.Add(item);
            return this;
        }
    }
}
#nullable disable
