using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility;
using Unity2Debug.Common.Utility.Tools;

namespace Unity2Debug.Common.Automation
{
    internal class PatchingAutomator
    {
        private readonly DebugSettings _debugSettings;
        private readonly DecompileSettings _decompileSettings;
        private readonly ILogger _logger;
        public PatchingAutomator(DebugSettings debugSettings, DecompileSettings decompileSettings, ILogger logger)
        {
            _debugSettings = debugSettings;
            _decompileSettings = decompileSettings;
            _logger = logger;
        }

        public void Start()
        {
            DoCreateSteamAppIdTxt();

            foreach (var assembly in _decompileSettings.AssemblyPaths)
            {
                if (string.IsNullOrEmpty(assembly) || !File.Exists(assembly))
                {
                    _logger.Error($"Could not patch {assembly}, file not found. Skipping...");
                    continue;
                }

                var outputFile = _debugSettings.ToDebugAssemblyPath(assembly);
                //DoDe4Dot(assembly, outputFile);
                DoCreateINI(assembly, outputFile);
            }
        }
        private void DoDe4Dot(string assembly, string outputFile)
        {
            _logger.Log($"Patching {assembly} to point at proper PDB");

            int result = Tools.De4dot(assembly, outputFile);

            if (result != 0)
            {
                _logger.Error($"Could not perform De4Dot on {assembly}");
                throw new InvalidOperationException();
            }

            _logger.Log($"Success. Deleting dummy PDB");

            var outDir = Path.GetDirectoryName(outputFile);

            if (!Directory.Exists(outDir))
                throw new DirectoryNotFoundException();

            var pdb = Path.Combine(outDir, $"{Path.GetFileNameWithoutExtension(outputFile)}.pdb");

            if (!File.Exists(pdb))
                throw new FileNotFoundException();

            File.Delete(pdb.LongPath());
        }

        public void DoCreateINI(string assembly, string outputFile)
        {
            _logger.Log($"Generating debug INI for {assembly}.");

            var outDir = Path.GetDirectoryName(outputFile);

            if (!Directory.Exists(outDir))
                throw new DirectoryNotFoundException();

            var ini = Path.Combine(outDir, $"{Path.GetFileNameWithoutExtension(outputFile)}.ini");

            File.WriteAllText(ini.LongPath(), "[.NET Framework Debugging Control]\r\nGenerateTrackingInfo=1\r\nAllowOptimize=0");
        }

        public void DoCreateSteamAppIdTxt()
        {
            _logger.Log("Creating steam_appid.txt");
            var path = Path.Combine(_debugSettings.DebugOutputPath, "steam_appid.txt");
            File.WriteAllText(path, _debugSettings.SteamAppId);
        }
    }
}
