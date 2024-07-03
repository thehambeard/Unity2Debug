using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Common.Automation
{
    public class UnityAutomator
    {
        private readonly DebugSettings _debugSettings;
        private readonly ILogger _logger;
        private readonly string _dataDirectory;

        private string? _unityMonoPath;

        public UnityAutomator(DebugSettings debugSettings, ILogger logger)
        {
            _debugSettings = debugSettings;
            _logger = logger;
            _dataDirectory = GetDataDirectory();
        }

        public void Start()
        {
            ReplaceWithDevelopmentMono();
            PatchBootConfig();
        }

        private void Copy(string source, string destination)
        {
            if (string.IsNullOrEmpty(_unityMonoPath) || !Directory.Exists(_unityMonoPath)) return;

            _logger.Log($"Copying {source}");

            var sourcePath = Path.Combine(
                _unityMonoPath,
                source);

            if (!File.Exists(sourcePath))
            {
                _logger.Error($"File {sourcePath} could not be found.");
                throw new FileNotFoundException();
            }

            File.Copy(
                sourcePath.LongPath(),
                destination.LongPath(),
                true);
        }

        public void ReplaceWithDevelopmentMono()
        {
            _unityMonoPath = UnityTools.GetUnityMonoPath(_debugSettings.UnityInstallPath.EnsureSeparator());

            if (!Directory.Exists(_unityMonoPath))
            {
                _logger.Error("Could not find Developement Mono Path!");
                throw new DirectoryNotFoundException();
            }

            Copy(UnityConstants.DEV_PLAYER_FILE, _debugSettings.ToDebugAssemblyPath(_debugSettings.RetailGameExe));
            Copy(UnityConstants.DEV_LIBRARY_FILE, Path.Combine(_debugSettings.DebugOutputPath, UnityConstants.DEV_LIBRARY_FILE));
            Copy(UnityConstants.DEV_WINPIX_FILE, Path.Combine(_debugSettings.DebugOutputPath, UnityConstants.DEV_WINPIX_FILE));
            Copy(UnityConstants.DEV_MONO_FILE, Path.Combine(_debugSettings.DebugOutputPath, UnityConstants.DEV_MONO_FILE));
        }

        public string GetDataDirectory()
        {
            var name = Path.GetFileNameWithoutExtension(_debugSettings.RetailGameExe);
            return Path.Combine(_debugSettings.DebugOutputPath, $"{name}_Data");
        }

        public string FindBootConfig()
        {
            if (Directory.Exists(_dataDirectory))
            {
                var bootConfig = Path.Combine(_dataDirectory, UnityConstants.BOOTCONFIG_FILENAME);
                if (File.Exists(bootConfig))
                    return bootConfig;
            }

            _logger.Warn("Unable to find boot.config in usual place. Attempting to search for it.");

            var files = Directory.GetFiles(_dataDirectory, "boot.config", SearchOption.AllDirectories);

            if (files.Length == 1)
                return files[0];
            else if (files.Length == 0)
                _logger.Error("Unable to find boot.config.");
            else
                _logger.Error("More than one boot.config found.");

            return string.Empty;
        }

        public void PatchBootConfig()
        {
            var bootFilePath = FindBootConfig();

            if (string.IsNullOrEmpty(bootFilePath))
                throw new FileNotFoundException();

            bool appendWaitDebug = true;
            bool appendPlayerConnect = true;

            var lines = File.ReadAllLines(bootFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(UnityConstants.BOOTCONFIG_CONNECT_DEBUG))
                {
                    lines[i] = $"{UnityConstants.BOOTCONFIG_CONNECT_DEBUG}1";
                    appendPlayerConnect = false;
                }
                else if (lines[i].StartsWith(UnityConstants.BOOTCONFIG_WAIT_DEBUG))
                {
                    lines[i] = $"{UnityConstants.BOOTCONFIG_WAIT_DEBUG}1";
                    appendWaitDebug = false;
                }
            }

            if (appendWaitDebug)
                lines = lines.AddTo($"{UnityConstants.BOOTCONFIG_WAIT_DEBUG}1");

            if (appendPlayerConnect)
                lines = lines.AddTo($"{UnityConstants.BOOTCONFIG_CONNECT_DEBUG}1");

            File.WriteAllLines(bootFilePath, lines);
        }
    }
}
