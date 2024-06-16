namespace Unity2Debug.Common.SettingsService
{
    [Serializable]
    public class SettingProfile
    {
        public string Name { get; set; }
        public DecompileSettings DecompileSettings { get; set; }
        public DebugSettings DebugSettings { get; set; }

        public SettingProfile(string name, DecompileSettings decompileSettings, DebugSettings debugSettings)
        {
            Name = name;
            DecompileSettings = decompileSettings;
            DebugSettings = debugSettings;
        }
    }
}
