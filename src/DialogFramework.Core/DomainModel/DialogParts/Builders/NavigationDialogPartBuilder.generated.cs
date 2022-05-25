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

namespace DialogFramework.Core.DomainModel.DialogParts.Builders
{
#nullable enable
    public partial class NavigationDialogPartBuilder
    {
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

        public string NavigateToId
        {
            get
            {
                return _navigateToIdDelegate.Value;
            }
            set
            {
                _navigateToIdDelegate = new (() => value);
            }
        }

        public DialogFramework.Abstractions.DomainModel.DialogParts.INavigationDialogPart Build()
        {
            #pragma warning disable CS8604 // Possible null reference argument.
            return new DialogFramework.Core.DomainModel.DialogParts.NavigationDialogPart(Id, NavigateToId);
            #pragma warning restore CS8604 // Possible null reference argument.
        }

        public NavigationDialogPartBuilder WithId(System.Func<string> idDelegate)
        {
            _idDelegate = new (idDelegate);
            return this;
        }

        public NavigationDialogPartBuilder WithId(string id)
        {
            Id = id;
            return this;
        }

        public NavigationDialogPartBuilder WithNavigateToId(System.Func<string> navigateToIdDelegate)
        {
            _navigateToIdDelegate = new (navigateToIdDelegate);
            return this;
        }

        public NavigationDialogPartBuilder WithNavigateToId(string navigateToId)
        {
            NavigateToId = navigateToId;
            return this;
        }

        public NavigationDialogPartBuilder()
        {
            #pragma warning disable CS8603 // Possible null reference return.
            _idDelegate = new (() => string.Empty);
            _navigateToIdDelegate = new (() => string.Empty);
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public NavigationDialogPartBuilder(DialogFramework.Abstractions.DomainModel.DialogParts.INavigationDialogPart source)
        {
            if (source == null)
            {
                throw new System.ArgumentNullException("source");
            }
            _idDelegate = new (() => source.Id);
            _navigateToIdDelegate = new (() => string.Empty);
        }

        private System.Lazy<string> _idDelegate;

        private System.Lazy<string> _navigateToIdDelegate;
    }
#nullable restore
}
