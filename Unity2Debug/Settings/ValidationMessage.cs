using CommunityToolkit.Mvvm.Messaging.Messages;
using FluentValidation.Results;

namespace Unity2Debug.Settings
{
    public class ValidationMessage : ValueChangedMessage<List<ValidationFailure>>
    {
        public ValidationMessage(List<ValidationFailure> failures) : base(failures)
        {
        }
    }
}
