using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
    public partial class DebugCopySetupVM : ViewModelBase
    {
        [ObservableProperty]
        private string? _selectedSymLink;

        [ObservableProperty]
        private string? _selectedExclude;

        public DebugCopySetupVM(TextBoxLogger logger, IDialogService dialogService, ObservableProfiles profiles) : base(logger, dialogService, profiles)
        {
            NotifyAllChanged();
        }

        public override void NotifyAllChanged() => Profiles?.CurrentProfile?.DebugSettings.NotifyAllChanged();
        public override void ValidateProfile() => Profiles?.CurrentProfile?.DebugSettings.CheckIsValid();


        [RelayCommand]
        private void SelectRetailExeFile()
        {
            if (Profiles.CurrentProfile == null) return;

            var file = _dialogService.OpenFile("EXE files (*.exe)|*.exe");
            if (file != null)
                Profiles.CurrentProfile.DebugSettings.RetailGameExe = file;

            ValidateProfile();
        }

        [RelayCommand]
        private void SelectDebugOutputPath()
        {
            if (Profiles.CurrentProfile == null) return;

            var path = _dialogService.OpenFolder();
            if (path != null)
                Profiles.CurrentProfile.DebugSettings.DebugOutputPath = path;

            ValidateProfile();
        }

        [RelayCommand]
        private void AddExcludeSymlink()
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
                    var syms = Profiles.CurrentProfile.DebugSettings.ExcludeFilters.ToList();
                    Profiles.CurrentProfile.DebugSettings.ExcludeFilters = [.. syms.Union(vm.Filters)];
                }
            });

            NotifyAllChanged();
        }

        [RelayCommand]
        private void RemoveExcludePath()
        {
            if (Profiles.CurrentProfile == null || SelectedExclude == null) return;

            if (Profiles.CurrentProfile.DebugSettings.ExcludeFilters.Contains(SelectedExclude))
                Profiles.CurrentProfile.DebugSettings.ExcludeFilters.Remove(SelectedExclude);

            NotifyAllChanged();
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

            NotifyAllChanged();
        }

        [RelayCommand]
        private void RemoveSymlink()
        {
            if (Profiles.CurrentProfile == null || SelectedSymLink == null) return;

            if (Profiles.CurrentProfile.DebugSettings.SymLinks.Contains(SelectedSymLink))
                Profiles.CurrentProfile.DebugSettings.SymLinks.Remove(SelectedSymLink);

            NotifyAllChanged();
        }

        [RelayCommand]
        private void SelectUnityInstallPath()
        {
            if (Profiles.CurrentProfile == null) return;

            var path = _dialogService.OpenFolder();

            if (path != null)
                Profiles.CurrentProfile.DebugSettings.UnityInstallPath = path;

            ValidateProfile();
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

            ValidateProfile();
        }
    }
}
