using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Unity2Debug.Dialogs.ViewModel;
using Unity2Debug.DialogService;
using Unity2Debug.Logging;
using Unity2Debug.Navigation;

namespace Unity2Debug.Pages.ViewModel
{
    public partial class InitialSetupVM : ViewModelBase
    {
        [ObservableProperty]
        private int _assemblySelectedIndex;

        [ObservableProperty]
        private string? _profileComboBoxText;

        public InitialSetupVM(TextBoxLogger logger, IDialogService dialogService) : base(logger, dialogService)
        {
            NotifyAllChanged();
        }

        public override void NotifyAllChanged() => Profiles?.CurrentProfile?.DecompileSettings.NotifyAllChanged();
        public override void ValidateProfile() => Profiles?.CurrentProfile?.DecompileSettings.CheckIsValid();

        [RelayCommand]
        private void AddProfile()
        {
            string? text = ProfileComboBoxText;
            if (!string.IsNullOrEmpty(text))
            {
                Profiles.AddProfile(text);
                Profiles.SelectProfile(text);
                ProfileComboBoxText = Profiles?.CurrentProfile?.Name;
                NotifyAllChanged();
            }
        }

        [RelayCommand]
        private void AutoAddProfile()
        {
            Profiles.Save();
            _dialogService.ShowDialog(new LoadDefaultsVM());
            Profiles = new(Common.SettingsService.Settings.Instance);
            NotifyAllChanged();
        }

        [RelayCommand]
        private void RemoveProfileButtonClick()
        {
            Profiles.RemoveProfile();
            Profiles.CurrentProfileIndex = 0;
            ProfileComboBoxText = Profiles?.CurrentProfile?.Name;
            NotifyAllChanged();
        }

        [RelayCommand]
        private void NextButtonClick()
        {
            if (Profiles.CurrentProfile == null || !Profiles.CurrentProfile.DecompileSettings.CheckIsValid()) return;

            Profiles.Save();

            if (Profiles.CurrentProfile.DebugSettings.CreateDebugCopy)
                NavigationService.Instance?.NavigateTo(new DebugCopySetup(Profiles));
            else
                NavigationService.Instance?.NavigateTo(new Processing(Profiles));
        }

        [RelayCommand]
        private void SelectOutputDirectory()
        {
            if (Profiles.CurrentProfile == null) return;

            string? folderPath = _dialogService.OpenFolder();
            if (folderPath != null)
                Profiles.CurrentProfile.DecompileSettings.OutputDirectory = folderPath;

            ValidateProfile();
        }

        [RelayCommand]
        private void SelectAssemblyFiles()
        {
            if (Profiles.CurrentProfile == null) return;

            List<string> filePaths = _dialogService.OpenFiles("DLL files (*.dll)|*.dll|All files (*.*)|*.*");
            List<string> current = [.. Profiles.CurrentProfile.DecompileSettings.AssemblyPaths];

            foreach (string filePath in filePaths)
                current.Add(filePath);

            Profiles.CurrentProfile.DecompileSettings.AssemblyPaths = [.. current];

            NotifyAllChanged();
        }

        [RelayCommand]
        private void RemoveAssemblyFile(int index)
        {
            if (Profiles.CurrentProfile == null) return;

            if (index >= 0 && index < Profiles.CurrentProfile.DecompileSettings.AssemblyPaths.Count)
                Profiles.CurrentProfile.DecompileSettings.AssemblyPaths.RemoveAt(index);

            NotifyAllChanged();
        }
    }
}
