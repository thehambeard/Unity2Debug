using CommunityToolkit.Mvvm.ComponentModel;
using Unity2Debug.Logging;

namespace Unity2Debug.Dialogs.ViewModel
{
    public partial class LoadDefaultsVM : ViewModelBase
    {
        [ObservableProperty]
        private TextBoxLogger? _logger;

        public void Load()
        {
            Common.SettingsService.Settings.Instance.GenerateDefaultProfiles(Logger);
        }
    }
}
