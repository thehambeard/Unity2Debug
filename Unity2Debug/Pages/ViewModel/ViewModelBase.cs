using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation.Results;
using Unity2Debug.DialogService;
using Unity2Debug.Logging;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages.ViewModel
{
    public partial class ViewModelBase<TViewModel> : ObservableObject
    {
        protected readonly RichTextBoxLogger _logger;
        protected readonly IDialogService _dialogService;

        public ViewModelBase(RichTextBoxLogger logger, IDialogService dialogService)
        {
            _logger = logger;
            _dialogService = dialogService;
        }
    }

    public partial class ViewModelBase<TViewModel, TSettings> :
        ViewModelBase<TViewModel>
    {
        [ObservableProperty]
        private ObservableProfiles _profiles;

        public ViewModelBase(RichTextBoxLogger logger, IDialogService dialogService, ObservableProfiles? profiles = null) : base(logger, dialogService)
        {
            if (profiles == null)
            {
                Profiles = new ObservableProfiles(Common.SettingsService.Settings.Load())
                {
                    CurrentProfileIndex = 0
                };
            }
            else
                Profiles = profiles;

            WeakReferenceMessenger.Default.Register<ValidationMessage>(this, (r, m) =>
            {
                LogErrors(m.Value);
            });
        }

        public void LogErrors(List<ValidationFailure> errors)
        {
            _logger.Clear();

            foreach (var error in errors)
                _logger.LogValidation(error);
        }
    }
}
