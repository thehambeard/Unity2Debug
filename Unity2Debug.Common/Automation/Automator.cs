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
        private readonly CancellationToken _cancellationToken;
        private readonly DecompileSettings _decompileSettings;
        private readonly DebugSettings _debugSettings;
        private readonly IProgress<DecompilationProgress> _decompilationProgress;

        public Automator(ILogger logger, DecompileSettings decompileSettings, DebugSettings debugSettings, CancellationToken cancellationToken, IProgress<DecompilationProgress> decompilationProgress)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
            _decompileSettings = decompileSettings;
            _debugSettings = debugSettings;
            _decompilationProgress = decompilationProgress;
        }

        public void Start()
        {
            try
            {
                if (!PreStartSettingChecks())
                    throw new InvalidOperationException();

                var timer = new Stopwatch();
                timer.Start();

                if (_debugSettings.CreateDebugCopy)
                {
                    CreateDebug();

                    //var paths = _debugSettings.ToDebugAssemblyPaths(_decompileSettings.AssemblyPaths);
                    //if (paths == null)
                    //{
                    //    _logger.Error("Could not generate debug assembly paths.");
                    //    throw new NullReferenceException();
                    //}

                    //Task.Run(() => DecompileAsync(paths)).Wait();

                    Task.Run(() => DecompileAsync(_decompileSettings.AssemblyPaths)).Wait();

                    UnityAutomator unityAutomator = new(_debugSettings, _logger);
                    unityAutomator.Start();
                }
                else
                {
                    Task.Run(() => DecompileAsync(_decompileSettings.AssemblyPaths)).Wait();
                }

                _logger.Log("Processing Complete!");
                timer.Stop();

                _logger.Log($"Processing took {timer.Elapsed.TotalSeconds} seconds to complete.");
            }
            catch (Exception ex)
            {
                _logger.Error("Processing Failed.");
                _logger.Error(ex);
            }
        }

        public void ResetProgress()
        {
            var p = new DecompilationProgress() { Title = "RESET", UnitsCompleted = 0, TotalUnits = 1 };
            _decompilationProgress.Report(p);
        }

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
                    _decompilationProgress
                    /*_debugSettings.CreateDebugCopy*/));

                if (project != null)
                {
                    projects.Add(project);

                    //if (_debugSettings.CreateDebugCopy)
                    //{
                    //    _logger.Log("Copying PDB to Debug Directory...");

                    //    var pdb = Path.ChangeExtension(project.FilePath, ".pdb");
                    //    var name = Path.GetFileNameWithoutExtension(_debugSettings.RetailGameExe);
                    //    var path = Path.Combine(_debugSettings.DebugOutputPath, $@"{name}_Data\Managed", Path.GetFileName(pdb));

                    //    File.Copy(pdb, path, true);
                    //}
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

        private void CreateDebug()
        {
            CopyGameAutomator copyAutomator = new(_debugSettings, _decompileSettings, _logger, _cancellationToken);
            Task.Run(copyAutomator.CopyGameAsync).Wait();

            PatchingAutomator patchingAutomator = new(_debugSettings, _decompileSettings, _logger);
            patchingAutomator.Start();
        }
    }
}
