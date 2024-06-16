using CommunityToolkit.Mvvm.ComponentModel;

namespace Unity2Debug.Settings
{
    public partial class ObservableProfile : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ObservableDecompileSettings _decompileSettings;

        [ObservableProperty]
        private ObservableDebugSettings _debugSettings;

        public ObservableProfile(string name, ObservableDecompileSettings? decompileSettings = null, ObservableDebugSettings? debugSettings = null)
        {
            Name = name;
            DecompileSettings = decompileSettings ?? new();
            DebugSettings = debugSettings ?? new();
        }
    }
}
