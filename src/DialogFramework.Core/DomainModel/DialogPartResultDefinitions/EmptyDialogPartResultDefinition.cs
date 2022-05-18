using System.Collections.Generic;
using System.Linq;
using CrossCutting.Common;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions
{
    public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
    {
        public string Id => string.Empty;
        public string Title => string.Empty;
        public ResultValueType ValueType => ResultValueType.None;

        public ValueCollection<IDialogPartResultDefinitionValidator> Validators => new();

        public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                             IDialogPart dialogPart,
                                                             IEnumerable<IDialogPartResult> dialogPartResults)
            => Enumerable.Empty<IDialogValidationResult>();
    }
}
