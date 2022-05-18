using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;

namespace DialogFramework.Core.DomainModel.DialogParts
{
    public record MessageDialogPart : IMessageDialogPart
    {
        public MessageDialogPart(string id,
                                 string heading,
                                 string message,
                                 IDialogPartGroup group)
        {
            Id = id;
            Heading = heading;
            Message = message;
            Group = group;
        }

        public string Message { get; }
        public string Heading { get; }
        public IDialogPartGroup Group { get; }
        public string Id { get; }
        public DialogState State => DialogState.InProgress;
    }
}
