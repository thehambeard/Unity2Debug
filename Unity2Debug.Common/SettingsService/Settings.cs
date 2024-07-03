using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService.Validators;
using Unity2Debug.Common.Utility;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Common.SettingsService
{
    public class Settings
    {
        private static Settings? _instance;

        public const string LOCAL_APP_DATA_FOLDER = "Unity2Debug";
        public const string SETTINGS_FILE_NAME = "settings.json";
        public const string DEFAULTS_FILE_NAME = "defaults.json";
        public Dictionary<string, SettingProfile> Profiles { get; private set; }
        public static Settings Instance => _instance ?? new();

        public Settings()
        {
            _instance = this;
            Profiles = [];
        }

        public bool TryAddProfile(string profileName)
        {
            bool result;
            DebugSettings debug = new();

            if (Directory.Exists(UnityConstants.UNITY_DEFAULT_BASE))
                debug.UnityInstallPath = UnityConstants.UNITY_DEFAULT_BASE;

            if (result = !Profiles.ContainsKey(profileName))
                Profiles.Add(profileName, new(profileName, new(), debug));

            return result;
        }

        public bool TryAddProfile(string profileName, SettingProfile profile)
        {
            bool result;

            if (result = TryAddProfile(profileName))
                Profiles[profileName] = profile;

            return result;
        }

        public bool TryAddProfile(string profileName, DecompileSettings decompileSettings, DebugSettings debugSettings)
        {
            bool result;

            if (result = TryAddProfile(profileName))
                Profiles[profileName] = new(profileName, decompileSettings, debugSettings);

            return result;
        }

        public bool TryRemoveProfile(string profileName)
        {
            bool result;
            if (result = Profiles.ContainsKey(profileName))
                Profiles.Remove(profileName);

            return result;
        }

        public static string? GetSettingsFilePath() => GetAppLocalFilePath(SETTINGS_FILE_NAME);
        public static string? GetDefaultsFilePath() => GetAppLocalFilePath(DEFAULTS_FILE_NAME, false);
        private static string? GetAppLocalFilePath(string filePath, bool removeAppPathCopy = true)
        {

            try
            {
                var localAppDataPath = Path.Combine
                    (
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        LOCAL_APP_DATA_FOLDER
                    );

                if (!string.IsNullOrEmpty(localAppDataPath) && !Directory.Exists(localAppDataPath))
                    Directory.CreateDirectory(localAppDataPath);

                var localAppDataFilePath = Path.Combine
                    (
                        localAppDataPath,
                        filePath
                    );

                if (File.Exists(localAppDataFilePath))
                    return localAppDataFilePath;

                var applicationPath =
                    Path.Combine
                    (
                        AppDomain.CurrentDomain.BaseDirectory,
                        filePath
                    );

                if (File.Exists(applicationPath))
                {
                    if (!File.Exists(localAppDataPath))
                        File.Copy(applicationPath, localAppDataFilePath, true);

                    if (removeAppPathCopy)
                        File.Delete(applicationPath);
                }

                return localAppDataFilePath;
            }
            catch
            {
                throw;
            }
        }


        public void Save(Dictionary<string, SettingProfile> profiles)
        {
            try
            {
                var file = GetSettingsFilePath();
                if (!string.IsNullOrEmpty(file))
                {
                    Profiles = profiles;
                    File.WriteAllText(file, Json.ToJSON(Profiles));
                }
            }
            catch
            {
                throw;
            }
        }

        public void Save() => Save(Profiles);

        public static bool TryLoad(out Settings? settings)
        {
            var file = GetSettingsFilePath();
            settings = null;

            try
            {
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var profiles = Json.FromJSON<Dictionary<string, SettingProfile>>(json);
                    if (profiles != null && profiles.Count > 0)
                    {
                        settings = new()
                        {
                            Profiles = profiles
                        };
                    }
                }
            }
            catch
            {
                throw;
            }

            return settings != null;
        }

        public void GenerateDefaultProfiles(ILogger? logger = null)
        {
            logger?.Log("Attempting to add defaults...");

            var file = GetDefaultsFilePath();

            if (!File.Exists(file))
            {
                logger?.Warn($"{DEFAULTS_FILE_NAME} does not exist. Writing an empty template.");

                var template = new DefaultProfile()
                {
                    Name = "UniqueName",
                    ExePath = "PathToGameExe.exe",
                    SteamAppId = "SteamId",
                    Symlinks = ["SymlinkFilter1", "SymlinkFilter2"],
                    AssemblyPaths = ["PathToAssembly1", "PathToAssembly2"]
                };

                try
                {
                    if (string.IsNullOrEmpty(file) || !Directory.Exists(Path.GetDirectoryName(file)))
                        throw new DirectoryNotFoundException();

                    File.WriteAllText(file, Json.ToJSON(new List<DefaultProfile>() { template }));
                }
                catch(Exception ex) 
                {
                    logger?.Error($"Failed to write {DEFAULTS_FILE_NAME}");
                    logger?.Error(ex);
                }
                return;
            }

            List<DefaultProfile>? defaults = [];

            try
            {
                defaults = Json.FromJSON<List<DefaultProfile>>(File.ReadAllText(file)) ?? throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                logger?.Error($"{DEFAULTS_FILE_NAME} is invalid.");
                logger?.Error(ex);
            }

            logger?.Log($"{defaults.Count} default profile{(defaults.Count > 1 ? "s" : string.Empty)} found...");

            foreach (var d in defaults)
            {
                var dpv = new DefaultProfileValidator([.. Profiles.Keys], () =>
                {
                    if (d.ExePath.EndsWith("Wrath.exe"))
                        return Path.Combine(OwlCatUtil.FindWrathPath(), "Wrath.exe");
                    if (d.ExePath.EndsWith("WH40KRT.exe"))
                        return Path.Combine(OwlCatUtil.FindRTPath(), "WH40KRT.exe");
                    if (d.ExePath.EndsWith("Kingmaker.exe"))
                        return Path.Combine(OwlCatUtil.FindKMPath(), "Kingmaker.exe");

                    return string.Empty;
                });

                var validation = dpv.Validate(d);

                if (!validation.IsValid)
                {
                    foreach (var failure in validation.Errors)
                        logger?.LogValidation(failure);

                    continue;
                }

                logger?.Log($"Added {d.Name}");

                UnityTools.TryGetVaildUnityPath(out var unityPathAndVersion, UnityConstants.UNITY_DEFAULT_BASE, d.ExePath);

                Profiles.Add(
                d.Name,
                new(d.Name,
                new()
                {
                    AssemblyPaths = d.GetAbsoluteAssemblyPaths(),
                },
                new()
                {
                    RetailGameExe = d.ExePath,
                    CreateDebugCopy = true,
                    VerboseLogging = false,
                    SteamAppId = d.SteamAppId,
                    Symlinks = d.Symlinks,
                    UseSymlinks = true,
                    UnityInstallPath = unityPathAndVersion != null ? unityPathAndVersion.Value.path : string.Empty,
                    UnityVersion = unityPathAndVersion != null ? unityPathAndVersion.Value.version : string.Empty
                }));
            }
        }
    }
}
