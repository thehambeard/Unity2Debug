using Newtonsoft.Json;
using Unity2Debug.Common.SettingsService.Validators;
using Unity2Debug.Common.Utility;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Common.SettingsService
{
    public class DebugSettings : SettingsBase<DebugSettings, DebugSettingsValidator>
    {
        public string RetailGameExe { get; set; }
        public string SteamAppId { get; set; }
        public string DebugOutputPath { get; set; }
        public string UnityInstallPath { get; set; }
        public string UnityVersion { get; set; }
        public bool UseSymlinks { get; set; }
        public List<string> Symlinks { get; set; }
        public List<string> ExcludeFilters { get; set; }
        public bool CreateDebugCopy { get; set; }
        public bool VerboseLogging {  get; set; }

        public DebugSettings()
        {
            RetailGameExe = string.Empty;
            SteamAppId = string.Empty;
            DebugOutputPath = string.Empty;
            UnityInstallPath = string.Empty;
            UnityVersion = string.Empty;
            UseSymlinks = false;
            Symlinks = [];
            ExcludeFilters = [];
        }

        public HashSet<string> GetFullSymlinkDirectories() => GetFullSymlinkPaths(Symlinks, false);
        public HashSet<string> GetFullExcludeDirectories() => GetFullSymlinkPaths(ExcludeFilters, false);
        public HashSet<string> GetFullSymlinkFileFilters() => GetFullSymlinkPaths(Symlinks, true);
        public HashSet<string> GetFullExcludeFileFilters() => GetFullSymlinkPaths(ExcludeFilters, true);

        private HashSet<string> GetFullSymlinkPaths(List<string> symlinks, bool getFiles)
        {
            var basePath = Path.GetDirectoryName(RetailGameExe)
                ?? throw new NullReferenceException();

            HashSet<string> result = [];

            foreach (var link in symlinks)
            {
                if (link.EndsWith('\\') && !getFiles)
                    result.Add(Path.Combine(basePath, link).TrimEnd('\\'));

                if(!link.EndsWith('\\') && getFiles)
                    result.Add(Path.Combine(basePath, link));
            }

            return result;
        }

        public string ToDebugAssemblyPath(string assemblyPath)
        {
            var baseDir = Path.GetDirectoryName(RetailGameExe);

            if (baseDir == null)
                return string.Empty;

            return assemblyPath.Replace(baseDir, DebugOutputPath);
        }

        public List<string> ToDebugAssemblyPaths(List<string> assemblyPaths)
        {
            if (assemblyPaths == null || assemblyPaths.Count == 0) return [];

            return assemblyPaths.Select(ToDebugAssemblyPath).ToList();
        }
    }
}
