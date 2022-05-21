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

namespace DialogFramework.UniversalModel.DomainModel.DialogParts.Builders
{
#nullable enable
    public partial class RedirectDialogPartBuilder
    {
        public DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder RedirectDialogMetadata
        {
            get
            {
                return _redirectDialogMetadataDelegate.Value;
            }
            set
            {
                _redirectDialogMetadataDelegate = new (() => value);
            }
        }

        public string Id
        {
            get
            {
                return _idDelegate.Value;
            }
            set
            {
                _idDelegate = new (() => value);
            }
        }

        public DialogFramework.Abstractions.DomainModel.Domains.DialogState State
        {
            get
            {
                return _stateDelegate.Value;
            }
            set
            {
                _stateDelegate = new (() => value);
            }
        }

        public DialogFramework.Abstractions.DomainModel.DialogParts.IRedirectDialogPart Build()
        {
            #pragma warning disable CS8604 // Possible null reference argument.
            return new DialogFramework.UniversalModel.DomainModel.DialogParts.RedirectDialogPart(RedirectDialogMetadata?.Build(), Id, State);
            #pragma warning restore CS8604 // Possible null reference argument.
        }

        public RedirectDialogPartBuilder WithId(System.Func<string> idDelegate)
        {
            _idDelegate = new (idDelegate);
            return this;
        }

        public RedirectDialogPartBuilder WithId(string id)
        {
            Id = id;
            return this;
        }

        public RedirectDialogPartBuilder WithRedirectDialogMetadata(DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder redirectDialogMetadata)
        {
            RedirectDialogMetadata = redirectDialogMetadata;
            return this;
        }

        public RedirectDialogPartBuilder WithRedirectDialogMetadata(System.Func<DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder> redirectDialogMetadataDelegate)
        {
            _redirectDialogMetadataDelegate = new (redirectDialogMetadataDelegate);
            return this;
        }

        public RedirectDialogPartBuilder WithState(DialogFramework.Abstractions.DomainModel.Domains.DialogState state)
        {
            State = state;
            return this;
        }

        public RedirectDialogPartBuilder WithState(System.Func<DialogFramework.Abstractions.DomainModel.Domains.DialogState> stateDelegate)
        {
            _stateDelegate = new (stateDelegate);
            return this;
        }

        public RedirectDialogPartBuilder()
        {
            #pragma warning disable CS8603 // Possible null reference return.
            _redirectDialogMetadataDelegate = new (() => default);
            _idDelegate = new (() => string.Empty);
            _stateDelegate = new (() => DialogFramework.Abstractions.DomainModel.Domains.DialogState.InProgress);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public RedirectDialogPartBuilder(DialogFramework.Abstractions.DomainModel.DialogParts.IRedirectDialogPart source)
        {
            if (source == null)
            {
                throw new System.ArgumentNullException("source");
            }
            _redirectDialogMetadataDelegate = new(() => new DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder(source.RedirectDialogMetadata));
            _idDelegate = new (() => source.Id);
            _stateDelegate = new (() => source.State);
        }

        private System.Lazy<DialogFramework.UniversalModel.DomainModel.Builders.DialogMetadataBuilder> _redirectDialogMetadataDelegate;

        private System.Lazy<string> _idDelegate;

        private System.Lazy<DialogFramework.Abstractions.DomainModel.Domains.DialogState> _stateDelegate;
    }
#nullable restore
}

