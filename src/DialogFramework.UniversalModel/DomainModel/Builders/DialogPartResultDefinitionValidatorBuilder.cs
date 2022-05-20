﻿using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.Builders
{
    public class DialogPartResultDefinitionValidatorBuilder
    {
        private readonly IDialogPartResultDefinitionValidator _validator;

        public DialogPartResultDefinitionValidatorBuilder(IDialogPartResultDefinitionValidator validator)
        {
            _validator = validator;
        }

        public IDialogPartResultDefinitionValidator Build()
        {
            return _validator;
        }
    }
}