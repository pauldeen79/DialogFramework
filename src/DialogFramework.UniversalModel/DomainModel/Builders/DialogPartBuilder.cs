using System;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.UniversalModel.DomainModel.DialogParts.Builders;

namespace DialogFramework.UniversalModel.DomainModel.Builders
{
    public class DialogPartBuilder
    {
        private readonly QuestionDialogPartBuilder? _questionDialogPartBuilder;

        public DialogPartBuilder(IDialogPart dialogPart)
        {
            if (dialogPart is IQuestionDialogPart questionDialogPart)
            {
                _questionDialogPartBuilder = new QuestionDialogPartBuilder(questionDialogPart);
            }

            throw new ArgumentException("Dialogpart should be of type IQuestionDialogPart");
        }

        public DialogPartBuilder(QuestionDialogPartBuilder questionDialogPartBuilder)
        {
            _questionDialogPartBuilder = questionDialogPartBuilder;
        }

        public IDialogPart Build()
        {
            if (_questionDialogPartBuilder != null)
            {
                return _questionDialogPartBuilder.Build();
            }

            throw new NotSupportedException("No valid type was found to build. Only question dialog part is supported.");
        }
    }
}
