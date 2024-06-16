using Unity2Debug.Common.SettingsService.Validators;

namespace Unity2Debug.Common.SettingsService
{
    public class DecompileSettings : SettingsBase<DecompileSettings, DecompileSettingsValidator>
    {
        public string OutputDirectory { get; set; }
        public List<string> AssemblyPaths { get; set; }

        public DecompileSettings()
        {
            OutputDirectory = string.Empty;
            AssemblyPaths = [];
        }
    }
}
