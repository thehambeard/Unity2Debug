using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation.Results;
using Unity2Debug.Dialogs.ViewModel;
using Unity2Debug.DialogService;
using Unity2Debug.Logging;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages.ViewModel
{
    public partial class ViewModelBase : ObservableObject
    {
        protected readonly TextBoxLogger _logger;
        protected readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableProfiles _profiles;

        public ViewModelBase(TextBoxLogger logger, IDialogService dialogService, ObservableProfiles? profiles = null)
        {
            _logger = logger;
            _dialogService = dialogService;

            WeakReferenceMessenger.Default.Register<ValidationMessage>(this, (r, m) =>
            {
                LogErrors(m.Value);
            });

            if (profiles != null)
            {
                Profiles = profiles;
                return;
            }

            if (Common.SettingsService.Settings.TryLoad(out var profs) && profs != null)
            {
                Profiles = new ObservableProfiles(profs)
                {
                    CurrentProfileIndex = 0
                };
                return;
            }

            if (dialogService.ShowMessageBox("settings.json not found or failed to load. Load defaults.json?", "Settings Load Failure", true))
                dialogService.ShowDialog(new LoadDefaultsVM());

            Profiles = new ObservableProfiles(Common.SettingsService.Settings.Instance)
            {
                CurrentProfileIndex = 0
            }; 
        }

        public virtual void NotifyAllChanged() { }
        public virtual void ValidateProfile() { }

        public void LogErrors(List<ValidationFailure> errors)
        {
            _logger.Clear();

            foreach (var error in errors)
                _logger.LogValidation(error);
        }
    }
}
