using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Unity2Debug.Common.SettingsService;

namespace Unity2Debug.Settings
{
    public partial class ObservableSettingsBase<T> : ObservableObject where T : SettingsBase<T>, new()
    {
        public ObservableSettingsBase()
        {
        }

        public virtual T ToNonObservableSettings() => new();

        public bool IsValid
        {
            get
            {
                var failures = ToNonObservableSettings().Validate().Errors;
                WeakReferenceMessenger.Default.Send(new ValidationMessage(failures));
                return !ToNonObservableSettings().HasErrors;
            }
        }

        public void NotifyAllChanged() => OnPropertyChanged();
    }
}
