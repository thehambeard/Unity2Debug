using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility.Tools;
using Unity2Debug.Dialogs.ViewModel;
using Unity2Debug.DialogService;
using Unity2Debug.Logging;
using Unity2Debug.Navigation;
using Unity2Debug.Settings;

namespace Unity2Debug.Pages.ViewModel
{
    public partial class DebugCopySetupVM : ViewModelBase<DebugCopySetupVM, DebugSettings>
    {
        [ObservableProperty]
        private string? _selectedSymLink;

        [ObservableProperty]
        private string? _selectedExclude;

        public DebugCopySetupVM(RichTextBoxLogger logger, IDialogService dialogService, ObservableProfiles profiles) : base(logger, dialogService, profiles)
        {
        }

        [RelayCommand]
        private void SelectRetailExeFile()
        {
            if (Profiles.CurrentProfile == null) return;

            var file = _dialogService.OpenFile("EXE files (*.exe)|*.exe");
            if (file != null)
                Profiles.CurrentProfile.DebugSettings.RetailGameExe = file;
        }

        [RelayCommand]
        private void SelectDebugOutputPath()
        {
            if (Profiles.CurrentProfile == null) return;

            var path = _dialogService.OpenFolder();
            if (path != null)
                Profiles.CurrentProfile.DebugSettings.DebugOutputPath = path;
        }

        [RelayCommand]
        private void SelectExcludePath()
        {
            if (Profiles.CurrentProfile == null) return;

            var path = _dialogService.OpenFolder();
            if (path != null && !Profiles.CurrentProfile.DebugSettings.ExcludeDirectories.Contains(path))
                Profiles.CurrentProfile.DebugSettings.ExcludeDirectories.Add(path);
        }

        [RelayCommand]
        private void RemoveExcludePath()
        {
            if (Profiles.CurrentProfile == null || SelectedExclude == null) return;

            if (Profiles.CurrentProfile.DebugSettings.ExcludeDirectories.Contains(SelectedExclude))
                Profiles.CurrentProfile.DebugSettings.ExcludeDirectories.Remove(SelectedExclude);
        }

        [RelayCommand]
        private void SelectUnityInstallPath()
        {
            if (Profiles.CurrentProfile == null) return;

            var path = _dialogService.OpenFolder();
            if (path != null)
                Profiles.CurrentProfile.DebugSettings.UnityInstallPath = path;
        }

        [RelayCommand]
        private void AddSymlink()
        {
            if (Profiles.CurrentProfile == null) return;

            var p = Path.GetDirectoryName(Profiles.CurrentProfile.DebugSettings.RetailGameExe);

            if (p == null || !Directory.Exists(p))
                return;

            var vm = new SymlinkDialogVM()
            {
                BasePath = p
            };


            _dialogService.ShowDialog(vm, (result, vm) =>
            {
                if (result == true)
                {
                    var syms = Profiles.CurrentProfile.DebugSettings.SymLinks.ToList();
                    Profiles.CurrentProfile.DebugSettings.SymLinks = [.. syms.Union(vm.Filters)];
                }
            });
        }

        [RelayCommand]
        private void RemoveSymlink()
        {
            if (Profiles.CurrentProfile == null || SelectedSymLink == null) return;

            if (Profiles.CurrentProfile.DebugSettings.SymLinks.Contains(SelectedSymLink))
                Profiles.CurrentProfile.DebugSettings.SymLinks.Remove(SelectedSymLink);
        }

        [RelayCommand]
        private void NextButtonClick()
        {
            if (Profiles.CurrentProfile == null) return;

            Profiles.Save();

            NavigationService.Instance?.NavigateTo(new Processing(Profiles));
        }

        [RelayCommand]
        private void BackButtonClick() => NavigationService.Instance?.Back();

        [RelayCommand]
        private void FindSteamAppIdClick()
        {
            if (Profiles.CurrentProfile == null) return;

            var appId = UnityTools.GetSteamAppId(Profiles.CurrentProfile.DebugSettings.RetailGameExe);

            if (appId != null)
                Profiles.CurrentProfile.DebugSettings.SteamAppId = appId;
            else
                _logger.Error("Unable to find the AppId.");
        }
    }
}
