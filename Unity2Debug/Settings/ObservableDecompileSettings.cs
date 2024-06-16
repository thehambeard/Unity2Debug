using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Unity2Debug.Common.SettingsService;

namespace Unity2Debug.Settings
{
    public partial class ObservableDecompileSettings : ObservableSettingsBase<DecompileSettings>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private string _outputDirectory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsValid))]
        private ObservableCollection<string> _assemblyPaths = [];
        partial void OnAssemblyPathsChanged(ObservableCollection<string> value)
        {
            this.AssemblyPaths.CollectionChanged += (s, e) =>
            {
                base.OnPropertyChanged(nameof(IsValid));
            };
        }

        public ObservableDecompileSettings() : this(new())
        {
        }

        public ObservableDecompileSettings(DecompileSettings decompileSettings)
        {
            this.OutputDirectory = decompileSettings.OutputDirectory;
            this.AssemblyPaths = [.. decompileSettings.AssemblyPaths];
        }

        public override DecompileSettings ToNonObservableSettings()
        {
            return new()
            {
                OutputDirectory = this.OutputDirectory,
                AssemblyPaths = [.. this.AssemblyPaths]
            };
        }
    }
}
