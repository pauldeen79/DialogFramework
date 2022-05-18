using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.Core
{
    public class DialogContextFactory : IDialogContextFactory
    {
        public virtual bool CanCreate(IDialog dialog)
            => true;

        public IDialogContext Create(IDialog dialog)
            => new DialogContext(dialog);
    }
}
