using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Solution;
using System.Diagnostics;
using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility;

namespace Unity2Debug.Common.Automation
{
    public class Automator
    {
        private readonly ILogger _logger;
        private readonly DecompileSettings _decompileSettings;
        private readonly DebugSettings _debugSettings;
        private readonly IProgress<DecompilationProgress> _decompilationProgress;

        public Automator(ILogger logger, DecompileSettings decompileSettings, DebugSettings debugSettings, IProgress<DecompilationProgress> decompilationProgress)
        {
            _logger = logger;
            _decompileSettings = decompileSettings;
            _debugSettings = debugSettings;
            _decompilationProgress = decompilationProgress;
        }

        public async Task StartAsync()
        {
            try
            {
                if (!PreStartSettingChecks())
                    throw new InvalidOperationException();

                var timer = new Stopwatch();
                timer.Start();

                if (_debugSettings.CreateDebugCopy)
                {
                    await CreateDebugAsync();
                    await DecompileAsync(_decompileSettings.AssemblyPaths);

                    UnityAutomator unityAutomator = new(_debugSettings, _logger);
                    unityAutomator.Start();
                }
                else
                {
                    await DecompileAsync(_decompileSettings.AssemblyPaths);
                }

                _logger.Log("Processing Complete!");
                timer.Stop();

                _logger.Log($"Processing took {timer.Elapsed.TotalSeconds} seconds to complete.");

                _decompilationProgress.Report(new() { UnitsCompleted = 1, TotalUnits = 1 });
                _logger.Log("Finished. Can exit app now.");
            }
            catch (Exception ex)
            {
                _logger.Error("Processing Failed.");
                _logger.Error(ex);
            }
        }

        public void ResetProgress() => _decompilationProgress.Report(new() { Title = "RESET", UnitsCompleted = 0, TotalUnits = 1 });

        public bool PreStartSettingChecks()
        {
            bool valid = true;

            if (_debugSettings.CreateDebugCopy && _debugSettings.HasErrors)
            {
                valid = false;
                foreach (var error in _debugSettings.Validate().Errors)
                    _logger.LogValidation(error);
            }

            if (_decompileSettings.HasErrors)
            {
                valid = false;
                foreach (var error in _decompileSettings.Validate().Errors)
                    _logger.LogValidation(error);
            }

            return valid;
        }

        private async Task DecompileAsync(List<string> assemblyPaths)
        {
            List<ProjectItem> projects = [];

            var slnFilePath = Path.Combine(_decompileSettings.OutputDirectory, $"{Path.GetFileNameWithoutExtension(_debugSettings.RetailGameExe)}.sln");

            if (File.Exists(slnFilePath))
                File.Delete(slnFilePath.LongPath());

            foreach (var assembly in assemblyPaths)
            {
                if (!File.Exists(assembly))
                {
                    _logger.Error($"{assembly} does not exist for decompliation. Skipping...");
                    continue;
                }

                var outputDir = Path.Combine(_decompileSettings.OutputDirectory, Path.GetFileNameWithoutExtension(assembly));

                var project = await Task.Run(() => ILSpyAutomation.ILSpyAutomation.Decompile(
                    assembly,
                    outputDir,
                    new DecompileLogger(_logger),
                    _decompilationProgress,
                    _debugSettings.CreateDebugCopy));

                if (project != null)
                {
                    projects.Add(project);

                    if (_debugSettings.CreateDebugCopy)
                    {
                        _logger.Log("Copying PDB to Debug Directory...");

                        var pdb = Path.ChangeExtension(project.FilePath, ".pdb");
                        var name = Path.GetFileNameWithoutExtension(_debugSettings.RetailGameExe);
                        var path = Path.Combine(_debugSettings.DebugOutputPath, $@"{name}_Data\Managed", Path.GetFileName(pdb));

                        File.Copy(pdb, path, true);
                    }
                }

                ResetProgress();
            }

            _logger.Log("Decompilation Complete.");

            _logger.Log("Writing SLN file.");
            try
            {
                var path = Path.Combine(_decompileSettings.OutputDirectory, $"{Path.GetFileNameWithoutExtension(_debugSettings.RetailGameExe)}.sln");

                if (File.Exists(path))
                {
                    _logger.Warn("SLN already exists! Skipping...");
                    throw new InvalidOperationException();
                }
                SolutionCreator.WriteSolutionFile(path, projects);
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to create SLN file.");
            }
        }

        private async Task CreateDebugAsync()
        {
            CopyGameAutomator copyAutomator = new(_debugSettings, _decompileSettings, _logger, _decompilationProgress);
            await copyAutomator.NewCopyParaAsync();

            PatchingAutomator patchingAutomator = new(_debugSettings, _decompileSettings, _logger);
            patchingAutomator.Start();
        }
    }
}
