using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Settings
{
    public partial class ObservableDebugSettings : ObservableSettingsBase<DebugSettings>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _unityVersion;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _unityInstallPath;
        partial void OnUnityInstallPathChanged(string value) => UpdateUnityVersion();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _retailGameExe;
        partial void OnRetailGameExeChanged(string value) => UpdateUnityVersion();

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private ObservableCollection<string> _excludeFilters = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private bool _verboseLogging;

        public ObservableDebugSettings() : this(new())
        {
        }

        public void UpdateUnityVersion()
        {
            if (UnityTools.TryGetVaildUnityPath(out var valid, UnityInstallPath, RetailGameExe) && valid != null)
            {
                UnityInstallPath = valid.Value.path;
                UnityVersion = valid.Value.version;
            }
            else
                UnityVersion = string.Empty;
        }

        public ObservableDebugSettings(DebugSettings debugSettings)
        {
            this.UnityVersion = debugSettings.UnityVersion;
            this.UnityInstallPath = debugSettings.UnityInstallPath;
            this.RetailGameExe = debugSettings.RetailGameExe;
            this.SteamAppId = debugSettings.SteamAppId;
            this.DebugOutputPath = debugSettings.DebugOutputPath;
            this.CreateDebugCopy = debugSettings.CreateDebugCopy;
            this.IsDecompileOnly = !debugSettings.CreateDebugCopy;
            this.UseSymlinks = debugSettings.UseSymlinks;
            this.SymLinks = [.. debugSettings.Symlinks];
            this.ExcludeFilters = [.. debugSettings.ExcludeFilters];
            this.VerboseLogging = debugSettings.VerboseLogging;
            this.SymLinks.CollectionChanged += (s, e) =>
            {
                base.OnPropertyChanged(nameof(IsValid));
            };

            this.ExcludeFilters.CollectionChanged += (s, e) =>
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
                Symlinks = [.. this.SymLinks],
                ExcludeFilters = [.. this.ExcludeFilters],
                VerboseLogging = this.VerboseLogging
            };
        }
    }
}
