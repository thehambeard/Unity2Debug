namespace Unity2Debug.Common.SettingsService
{
    public class DefaultProfile
    {
        public string Name { get; set; } = string.Empty;
        public string ExePath { get; set; } = string.Empty;
        public string SteamAppId { get; set; } = string.Empty;
        public List<string> Symlinks { get; set; } = [];
        public List<string> AssemblyPaths { get; set; } = [];

        public bool CheckExePath(Func<string>? fallback = null)
        {
            if (!File.Exists(ExePath) && fallback != null)
                ExePath = fallback.Invoke();

            return File.Exists(ExePath);
        }

        public List<string> GetAbsoluteAssemblyPaths()
        {
            var basePath = Path.GetDirectoryName(ExePath);

            if (!Directory.Exists(basePath))
                return [];

            List<string> result = [];

            foreach (var assembly in AssemblyPaths)
                result.Add(Path.Combine(basePath, assembly));

            return result;
        }
    }
}
