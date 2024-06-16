using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.DialogService;
using Unity2Debug.Logging;
using Unity2Debug.Navigation;

namespace Unity2Debug.Pages.ViewModel
{
    public partial class InitialSetupVM : ViewModelBase<InitialSetupVM, DecompileSettings>
    {
        [ObservableProperty]
        private int _assemblySelectedIndex;

        [ObservableProperty]
        private string? _profileComboBoxText;

        public InitialSetupVM(RichTextBoxLogger logger, IDialogService dialogService) : base(logger, dialogService)
        {
        }

        [RelayCommand]
        private void AddProfile()
        {
            string? text = ProfileComboBoxText;
            if (!string.IsNullOrEmpty(text))
            {
                Profiles.AddProfile(text);
                Profiles.SelectProfile(text);
                ProfileComboBoxText = Profiles?.CurrentProfile?.Name;
            }
        }

        [RelayCommand]
        private void RemoveProfileButtonClick()
        {
            Profiles.RemoveProfile();
            Profiles.CurrentProfileIndex = 0;
            ProfileComboBoxText = Profiles?.CurrentProfile?.Name;
        }

        [RelayCommand]
        private void NextButtonClick()
        {
            if (Profiles.CurrentProfile == null) return;

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
        }

        [RelayCommand]
        private void RemoveAssemblyFile(int index)
        {
            if (Profiles.CurrentProfile == null) return;

            if (index >= 0 && index < Profiles.CurrentProfile.DecompileSettings.AssemblyPaths.Count)
                Profiles.CurrentProfile.DecompileSettings.AssemblyPaths.RemoveAt(index);
        }
    }
}
