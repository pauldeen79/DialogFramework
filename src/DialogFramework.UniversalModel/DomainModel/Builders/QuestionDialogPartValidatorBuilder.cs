using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel.Builders
{
    public class QuestionDialogPartValidatorBuilder
    {
        private readonly IQuestionDialogPartValidator _questionDialogPartValidator;

        public QuestionDialogPartValidatorBuilder(IQuestionDialogPartValidator questionDialogPartValidator)
            => _questionDialogPartValidator = questionDialogPartValidator;

        public IQuestionDialogPartValidator Build() => _questionDialogPartValidator;
    }
}
