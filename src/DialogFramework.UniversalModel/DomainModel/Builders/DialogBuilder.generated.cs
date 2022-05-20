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

namespace DialogFramework.UniversalModel.DomainModel.Builders
{
#nullable enable
    public partial class DialogBuilder
    {
        public DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder Metadata
        {
            get
            {
                return _metadataDelegate.Value;
            }
            set
            {
                _metadataDelegate = new (() => value);
            }
        }

        public System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder> Parts
        {
            get;
            set;
        }

        public DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.ErrorDialogPartBuilder ErrorPart
        {
            get
            {
                return _errorPartDelegate.Value;
            }
            set
            {
                _errorPartDelegate = new (() => value);
            }
        }

        public DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.AbortedDialogPartBuilder AbortedPart
        {
            get
            {
                return _abortedPartDelegate.Value;
            }
            set
            {
                _abortedPartDelegate = new (() => value);
            }
        }

        public DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.CompletedDialogPartBuilder CompletedPart
        {
            get
            {
                return _completedPartDelegate.Value;
            }
            set
            {
                _completedPartDelegate = new (() => value);
            }
        }

        public System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder> PartGroups
        {
            get;
            set;
        }

        public DialogBuilder AddPartGroups(params DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder[] partGroups)
        {
            if (partGroups != null)
            {
                PartGroups.AddRange(partGroups);
            }
            return this;
        }

        public DialogBuilder AddPartGroups(System.Collections.Generic.IEnumerable<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder> partGroups)
        {
            return AddPartGroups(partGroups.ToArray());
        }

        public DialogBuilder AddParts(params DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder[] parts)
        {
            if (parts != null)
            {
                Parts.AddRange(parts);
            }
            return this;
        }

        public DialogBuilder AddParts(System.Collections.Generic.IEnumerable<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder> parts)
        {
            return AddParts(parts.ToArray());
        }

        public DialogFramework.Abstractions.DomainModel.IDialog Build()
        {
            #pragma warning disable CS8604 // Possible null reference argument.
            return new DialogFramework.UniversalModel.DomainModel.Dialog(Metadata?.Build(), Parts.Select(x => x.Build()), ErrorPart?.Build(), AbortedPart?.Build(), CompletedPart?.Build(), PartGroups.Select(x => x.Build()));
            #pragma warning restore CS8604 // Possible null reference argument.
        }

        public DialogBuilder WithAbortedPart(DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.AbortedDialogPartBuilder abortedPart)
        {
            AbortedPart = abortedPart;
            return this;
        }

        public DialogBuilder WithAbortedPart(System.Func<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.AbortedDialogPartBuilder> abortedPartDelegate)
        {
            _abortedPartDelegate = new (abortedPartDelegate);
            return this;
        }

        public DialogBuilder WithCompletedPart(DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.CompletedDialogPartBuilder completedPart)
        {
            CompletedPart = completedPart;
            return this;
        }

        public DialogBuilder WithCompletedPart(System.Func<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.CompletedDialogPartBuilder> completedPartDelegate)
        {
            _completedPartDelegate = new (completedPartDelegate);
            return this;
        }

        public DialogBuilder WithErrorPart(DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.ErrorDialogPartBuilder errorPart)
        {
            ErrorPart = errorPart;
            return this;
        }

        public DialogBuilder WithErrorPart(System.Func<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.ErrorDialogPartBuilder> errorPartDelegate)
        {
            _errorPartDelegate = new (errorPartDelegate);
            return this;
        }

        public DialogBuilder WithMetadata(DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder metadata)
        {
            Metadata = metadata;
            return this;
        }

        public DialogBuilder WithMetadata(System.Func<DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder> metadataDelegate)
        {
            _metadataDelegate = new (metadataDelegate);
            return this;
        }

        public DialogBuilder()
        {
            Parts = new System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder>();
            PartGroups = new System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder>();
            #pragma warning disable CS8603 // Possible null reference return.
            _metadataDelegate = new (() => default);
            _errorPartDelegate = new (() => default);
            _abortedPartDelegate = new (() => default);
            _completedPartDelegate = new (() => default);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public DialogBuilder(DialogFramework.Abstractions.DomainModel.IDialog source)
        {
            if (source == null)
            {
                throw new System.ArgumentNullException("source");
            }
            Parts = new System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder>();
            PartGroups = new System.Collections.Generic.List<DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder>();
            _metadataDelegate = new(() => new DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder(source.Metadata));
            if (source.Parts != null) Parts.AddRange(source.Parts.Select(x => new DialogFramework.UniversalModel.DomainModel.Builders.DialogPartBuilder(x)));
            _errorPartDelegate = new(() => new DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.ErrorDialogPartBuilder(source.ErrorPart));
            _abortedPartDelegate = new(() => new DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.AbortedDialogPartBuilder(source.AbortedPart));
            _completedPartDelegate = new(() => new DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.CompletedDialogPartBuilder(source.CompletedPart));
            if (source.PartGroups != null) PartGroups.AddRange(source.PartGroups.Select(x => new DialogFramework.UniversalModel.DomainModel.Builders.DialogPartGroupBuilder(x)));
        }

        private System.Lazy<DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder> _metadataDelegate;

        private System.Lazy<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.ErrorDialogPartBuilder> _errorPartDelegate;

        private System.Lazy<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.AbortedDialogPartBuilder> _abortedPartDelegate;

        private System.Lazy<DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.CompletedDialogPartBuilder> _completedPartDelegate;
    }
#nullable restore
}
