﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace DialogFramework.Domain.Builders
{
    public partial class ClosedQuestionOptionBuilder
    {
        private ExpressionFramework.Domain.Builders.EvaluatableBuilder? _condition;

        private string _key;

        private string _displayName;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public ExpressionFramework.Domain.Builders.EvaluatableBuilder? Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<ExpressionFramework.Domain.Builders.EvaluatableBuilder>.Default.Equals(_condition!, value!);
                _condition = value;
                if (hasChanged) HandlePropertyChanged(nameof(Condition));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_key!, value!);
                _key = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Key));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_displayName!, value!);
                _displayName = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(DisplayName));
            }
        }

        public ClosedQuestionOptionBuilder(DialogFramework.Domain.ClosedQuestionOption source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _condition = source.Condition?.ToBuilder()!;
            _key = source.Key;
            _displayName = source.DisplayName;
        }

        public ClosedQuestionOptionBuilder()
        {
            _key = string.Empty;
            _displayName = string.Empty;
            SetDefaultValues();
        }

        public DialogFramework.Domain.ClosedQuestionOption Build()
        {
            return new DialogFramework.Domain.ClosedQuestionOption(Condition?.Build(), Key, DisplayName);
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder WithCondition(ExpressionFramework.Domain.Builders.EvaluatableBuilder? condition)
        {
            Condition = condition;
            return this;
        }

        public DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder WithKey(string key)
        {
            if (key is null) throw new System.ArgumentNullException(nameof(key));
            Key = key;
            return this;
        }

        public DialogFramework.Domain.Builders.ClosedQuestionOptionBuilder WithDisplayName(string displayName)
        {
            if (displayName is null) throw new System.ArgumentNullException(nameof(displayName));
            DisplayName = displayName;
            return this;
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class DialogBuilder
    {
        private string _id;

        private string _definitionId;

        private System.Version _definitionVersion;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartResultBuilder> _results;

        private object? _context;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_id!, value!);
                _id = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Id));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string DefinitionId
        {
            get
            {
                return _definitionId;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_definitionId!, value!);
                _definitionId = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(DefinitionId));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Version DefinitionVersion
        {
            get
            {
                return _definitionVersion;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Version>.Default.Equals(_definitionVersion!, value!);
                _definitionVersion = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(DefinitionVersion));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartResultBuilder> Results
        {
            get
            {
                return _results;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartResultBuilder>>.Default.Equals(_results!, value!);
                _results = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Results));
            }
        }

        public object? Context
        {
            get
            {
                return _context;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(_context!, value!);
                _context = value;
                if (hasChanged) HandlePropertyChanged(nameof(Context));
            }
        }

        public DialogBuilder(DialogFramework.Domain.Dialog source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _results = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartResultBuilder>();
            _id = source.Id;
            _definitionId = source.DefinitionId;
            _definitionVersion = source.DefinitionVersion;
            if (source.Results is not null) foreach (var item in source.Results.Select(x => x.ToBuilder())) _results.Add(item);
            _context = source.Context;
        }

        public DialogBuilder()
        {
            _results = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartResultBuilder>();
            _id = string.Empty;
            _definitionId = string.Empty;
            _definitionVersion = new System.Version(1, 0, 0)!;
            SetDefaultValues();
        }

        public DialogFramework.Domain.Dialog Build()
        {
            return new DialogFramework.Domain.Dialog(Id, DefinitionId, DefinitionVersion, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.DialogPartResult>(Results.Select(x => x.Build()!)), Context);
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogBuilder AddResults(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.DialogPartResultBuilder> results)
        {
            if (results is null) throw new System.ArgumentNullException(nameof(results));
            return AddResults(results.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogBuilder AddResults(params DialogFramework.Domain.Builders.DialogPartResultBuilder[] results)
        {
            if (results is null) throw new System.ArgumentNullException(nameof(results));
            foreach (var item in results) Results.Add(item);
            return this;
        }

        public DialogFramework.Domain.Builders.DialogBuilder WithId(string id)
        {
            if (id is null) throw new System.ArgumentNullException(nameof(id));
            Id = id;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogBuilder WithDefinitionId(string definitionId)
        {
            if (definitionId is null) throw new System.ArgumentNullException(nameof(definitionId));
            DefinitionId = definitionId;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogBuilder WithDefinitionVersion(System.Version definitionVersion)
        {
            if (definitionVersion is null) throw new System.ArgumentNullException(nameof(definitionVersion));
            DefinitionVersion = definitionVersion;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogBuilder WithContext(object? context)
        {
            Context = context;
            return this;
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class DialogDefinitionBuilder
    {
        private string _id;

        private string _name;

        private System.Version _version;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartSectionBuilder> _sections;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_id!, value!);
                _id = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Id));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_name!, value!);
                _name = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Name));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Version>.Default.Equals(_version!, value!);
                _version = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Version));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartSectionBuilder> Sections
        {
            get
            {
                return _sections;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartSectionBuilder>>.Default.Equals(_sections!, value!);
                _sections = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Sections));
            }
        }

        public DialogDefinitionBuilder(DialogFramework.Domain.DialogDefinition source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _sections = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartSectionBuilder>();
            _id = source.Id;
            _name = source.Name;
            _version = source.Version;
            if (source.Sections is not null) foreach (var item in source.Sections.Select(x => x.ToBuilder())) _sections.Add(item);
        }

        public DialogDefinitionBuilder()
        {
            _sections = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartSectionBuilder>();
            _id = string.Empty;
            _name = string.Empty;
            _version = new System.Version(1, 0, 0)!;
            SetDefaultValues();
        }

        public DialogFramework.Domain.DialogDefinition Build()
        {
            return new DialogFramework.Domain.DialogDefinition(Id, Name, Version, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.DialogPartSection>(Sections.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogDefinitionBuilder AddSections(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.DialogPartSectionBuilder> sections)
        {
            if (sections is null) throw new System.ArgumentNullException(nameof(sections));
            return AddSections(sections.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogDefinitionBuilder AddSections(params DialogFramework.Domain.Builders.DialogPartSectionBuilder[] sections)
        {
            if (sections is null) throw new System.ArgumentNullException(nameof(sections));
            foreach (var item in sections) Sections.Add(item);
            return this;
        }

        public DialogFramework.Domain.Builders.DialogDefinitionBuilder WithId(string id)
        {
            if (id is null) throw new System.ArgumentNullException(nameof(id));
            Id = id;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogDefinitionBuilder WithName(string name)
        {
            if (name is null) throw new System.ArgumentNullException(nameof(name));
            Name = name;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogDefinitionBuilder WithVersion(System.Version version)
        {
            if (version is null) throw new System.ArgumentNullException(nameof(version));
            Version = version;
            return this;
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class DialogPartSectionBuilder
    {
        private string _id;

        private ExpressionFramework.Domain.Builders.EvaluatableBuilder? _condition;

        private string _name;

        private System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartBuilder> _parts;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_id!, value!);
                _id = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Id));
            }
        }

        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public ExpressionFramework.Domain.Builders.EvaluatableBuilder? Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<ExpressionFramework.Domain.Builders.EvaluatableBuilder>.Default.Equals(_condition!, value!);
                _condition = value;
                if (hasChanged) HandlePropertyChanged(nameof(Condition));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_name!, value!);
                _name = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Name));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartBuilder> Parts
        {
            get
            {
                return _parts;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartBuilder>>.Default.Equals(_parts!, value!);
                _parts = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Parts));
            }
        }

        public DialogPartSectionBuilder(DialogFramework.Domain.DialogPartSection source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _parts = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartBuilder>();
            _id = source.Id;
            _condition = source.Condition?.ToBuilder()!;
            _name = source.Name;
            if (source.Parts is not null) foreach (var item in source.Parts.Select(x => x.ToBuilder())) _parts.Add(item);
        }

        public DialogPartSectionBuilder()
        {
            _parts = new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.Builders.DialogPartBuilder>();
            _id = string.Empty;
            _name = string.Empty;
            SetDefaultValues();
        }

        public DialogFramework.Domain.DialogPartSection Build()
        {
            return new DialogFramework.Domain.DialogPartSection(Id, Condition?.Build(), Name, new System.Collections.ObjectModel.ObservableCollection<DialogFramework.Domain.DialogPart>(Parts.Select(x => x.Build()!)));
        }

        partial void SetDefaultValues();

        public DialogFramework.Domain.Builders.DialogPartSectionBuilder AddParts(System.Collections.Generic.IEnumerable<DialogFramework.Domain.Builders.DialogPartBuilder> parts)
        {
            if (parts is null) throw new System.ArgumentNullException(nameof(parts));
            return AddParts(parts.ToArray());
        }

        public DialogFramework.Domain.Builders.DialogPartSectionBuilder AddParts(params DialogFramework.Domain.Builders.DialogPartBuilder[] parts)
        {
            if (parts is null) throw new System.ArgumentNullException(nameof(parts));
            foreach (var item in parts) Parts.Add(item);
            return this;
        }

        public DialogFramework.Domain.Builders.DialogPartSectionBuilder WithId(string id)
        {
            if (id is null) throw new System.ArgumentNullException(nameof(id));
            Id = id;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogPartSectionBuilder WithCondition(ExpressionFramework.Domain.Builders.EvaluatableBuilder? condition)
        {
            Condition = condition;
            return this;
        }

        public DialogFramework.Domain.Builders.DialogPartSectionBuilder WithName(string name)
        {
            if (name is null) throw new System.ArgumentNullException(nameof(name));
            Name = name;
            return this;
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
#nullable disable
