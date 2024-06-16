using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Settings
{
    public partial class ObservableDebugSettings : ObservableSettingsBase<DebugSettings>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private ObservableCollection<string> _unityVersions = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _unityVersion;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        [NotifyPropertyChangedFor(nameof(UnityVersions))]
        private string _unityInstallPath;
        partial void OnUnityInstallPathChanged(string value) => UpdateUnityVersions();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        [NotifyPropertyChangedFor(nameof(UnityVersions))]
        private string _retailGameExe;
        partial void OnRetailGameExeChanged(string value) => UpdateUnityVersions();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _steamAppId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _debugOutputPath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private bool _createDebugCopy;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private bool _isDecompileOnly;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private bool _useSymlinks;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private ObservableCollection<string> _symLinks = [];

        public ObservableDebugSettings() : this(new())
        {
        }

        public void UpdateUnityVersions()
        {
            if (Directory.Exists(UnityInstallPath))
            {
                UnityVersions = [.. UnityTools.GetUnityVersionsInPath(UnityInstallPath)];

                var version = UnityTools.GetUnityVersionFromAssembly(RetailGameExe);

                if (!string.IsNullOrEmpty(version) && UnityVersions.Contains($"{version}f1"))
                    UnityVersion = $"{version}f1";
            }
        }

        public ObservableDebugSettings(DebugSettings debugSettings)
        {
            this.UnityVersions = [.. debugSettings.UnityVersions];
            this.UnityVersion = debugSettings.UnityVersion;
            this.UnityInstallPath = debugSettings.UnityInstallPath;
            this.RetailGameExe = debugSettings.RetailGameExe;
            this.SteamAppId = debugSettings.SteamAppId;
            this.DebugOutputPath = debugSettings.DebugOutputPath;
            this.CreateDebugCopy = debugSettings.CreateDebugCopy;
            this.IsDecompileOnly = !debugSettings.CreateDebugCopy;
            this.UseSymlinks = debugSettings.UseSymlinks;
            this.SymLinks = [.. debugSettings.Symlinks];

            this.UnityVersions.CollectionChanged += (s, e) =>
            {
                base.OnPropertyChanged(nameof(IsValid));
            };

            this.SymLinks.CollectionChanged += (s, e) =>
            {
                base.OnPropertyChanged(nameof(IsValid));
            };
        }

        public override DebugSettings ToNonObservableSettings()
        {
            return new()
            {
                RetailGameExe = this.RetailGameExe,
                SteamAppId = this.SteamAppId,
                UnityVersion = this.UnityVersion,
                UnityInstallPath = this.UnityInstallPath,
                DebugOutputPath = this.DebugOutputPath,
                CreateDebugCopy = this.CreateDebugCopy,
                UseSymlinks = this.UseSymlinks,
                Symlinks = [.. this.SymLinks]
            };
        }
    }
}
