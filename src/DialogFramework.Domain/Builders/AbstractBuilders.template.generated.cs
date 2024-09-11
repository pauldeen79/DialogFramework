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
namespace DialogFramework.Domain.Builders
{
    public abstract partial class DialogPartBuilder<TBuilder, TEntity> : DialogPartBuilder
        where TEntity : DialogFramework.Domain.DialogPart
        where TBuilder : DialogPartBuilder<TBuilder, TEntity>
    {
        protected DialogPartBuilder(DialogFramework.Domain.DialogPart source) : base(source)
        {
        }

        protected DialogPartBuilder() : base()
        {
        }

        public override DialogFramework.Domain.DialogPart Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();

        public TBuilder WithId(string id)
        {
            if (id is null) throw new System.ArgumentNullException(nameof(id));
            Id = id;
            return (TBuilder)this;
        }

        public TBuilder WithCondition(ExpressionFramework.Domain.Builders.EvaluatableBuilder? condition)
        {
            Condition = condition;
            return (TBuilder)this;
        }

        public TBuilder WithTitle(string title)
        {
            if (title is null) throw new System.ArgumentNullException(nameof(title));
            Title = title;
            return (TBuilder)this;
        }
    }
    public abstract partial class DialogPartResultBuilder<TBuilder, TEntity> : DialogPartResultBuilder
        where TEntity : DialogFramework.Domain.DialogPartResult
        where TBuilder : DialogPartResultBuilder<TBuilder, TEntity>
    {
        protected DialogPartResultBuilder(DialogFramework.Domain.DialogPartResult source) : base(source)
        {
        }

        protected DialogPartResultBuilder() : base()
        {
        }

        public override DialogFramework.Domain.DialogPartResult Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();

        public TBuilder WithPartId(string partId)
        {
            if (partId is null) throw new System.ArgumentNullException(nameof(partId));
            PartId = partId;
            return (TBuilder)this;
        }
    }
    public abstract partial class ValidationRuleBuilder<TBuilder, TEntity> : ValidationRuleBuilder
        where TEntity : DialogFramework.Domain.ValidationRule
        where TBuilder : ValidationRuleBuilder<TBuilder, TEntity>
    {
        protected ValidationRuleBuilder(DialogFramework.Domain.ValidationRule source) : base(source)
        {
        }

        protected ValidationRuleBuilder() : base()
        {
        }

        public override DialogFramework.Domain.ValidationRule Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();
    }
}
#nullable disable
