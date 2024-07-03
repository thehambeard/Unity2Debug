using ICSharpCode.Decompiler;
using System.Diagnostics;
using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility;

namespace Unity2Debug.Common.Automation
{
    public class CopyGameAutomator
    {
        public DebugSettings DebugSettings { get; set; }
        public DecompileSettings DecompileSettings { get; set; }

        private readonly ILogger _logger;
        private readonly IProgress<DecompilationProgress> _progress;
        private readonly bool _verbose = false;
        public CopyGameAutomator(DebugSettings debugSettings, DecompileSettings decompileSettings, ILogger logger, IProgress<DecompilationProgress> progress)
        {
            DebugSettings = debugSettings;
            DecompileSettings = decompileSettings;
            _logger = logger;
            _progress = progress;
        }

        public async Task NewCopyParaAsync()
        {
            var timer = new Stopwatch();
            timer.Start();

            var baseInputPath = Path.GetDirectoryName(DebugSettings.RetailGameExe)
                ?? throw new NullReferenceException();

            HashSet<string> exludeDir = DebugSettings.GetFullExcludeDirectories();
            HashSet<string> symlinkPaths = [];

            if (DebugSettings.UseSymlinks)
                symlinkPaths = DebugSettings.GetFullSymlinkDirectories();

            _logger.Log("Starting Game Copy...");
            

            await Task.Run(() =>
            {
                try
                {

                    var copyDirs = Directory.GetDirectories(baseInputPath, "*", SearchOption.AllDirectories)
                        .Where(dir => !symlinkPaths.Any(symlink => dir.Contains(symlink))
                            && !exludeDir.Any(exclude => dir.Contains(exclude)));

                    _logger.Log($"Creating {copyDirs.Count()} Directories...");
                    _logger.Log($"Symlinking {symlinkPaths.Count} Directories...");

                    var progressStruct = new DecompilationProgress { UnitsCompleted = 0, TotalUnits = copyDirs.Count() + symlinkPaths.Count };
                    _progress.Report(progressStruct);

                    Parallel.ForEach(copyDirs, currentInputPath =>
                    {
                        if (_verbose) _logger.Log($"Creating {currentInputPath}");
                        Directory.CreateDirectory(currentInputPath.ToDebugAssemblyPath(baseInputPath, DebugSettings.DebugOutputPath));
                        progressStruct.UnitsCompleted++;
                        _progress.Report(progressStruct);
                    });


                    Parallel.ForEach(symlinkPaths, symlinkPath =>
                    {
                        if (_verbose) _logger.Log($"Symlinking {symlinkPath}");
                        Directory.CreateSymbolicLink(symlinkPath.ToDebugAssemblyPath(baseInputPath, DebugSettings.DebugOutputPath), symlinkPath);
                        progressStruct.UnitsCompleted++;
                        _progress.Report(progressStruct);
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to create directory {ex.Message}!");
                }
            });

            _progress.Report(new() { Title = "RESET", UnitsCompleted = 0, TotalUnits = 1 }); 

            HashSet<string> symlinkFilePaths = [];

            if (DebugSettings.UseSymlinks)
                symlinkFilePaths = DebugSettings.GetFullSymlinkFileFilters();

            FileMatcher matcher = new(symlinkFilePaths);
            FileMatcher excludeFileMatcher = new(DebugSettings.GetFullExcludeFileFilters());

            await Task.Run(() =>
            {

                try
                {
                    var files = Directory.GetFiles(baseInputPath, "*.*", SearchOption.AllDirectories)
                        .Where(dir => !symlinkPaths.Any(symlink => dir.Contains(symlink))
                            && !exludeDir.Any(exclude => dir.Contains(exclude)));

                    _logger.Log($"Copying {files.Count()} game files...");


                    var progressStruct = new DecompilationProgress { UnitsCompleted = 0, TotalUnits = files.Count() };
                    _progress.Report(progressStruct);

                    Parallel.ForEach(files, currentInputFile =>
                    {
                        if (excludeFileMatcher.Match(currentInputFile))
                        {
                            if (_verbose) _logger.Log($"Excluding {currentInputFile}");
                        }
                        else if (matcher.Match(currentInputFile))
                        {
                            if (_verbose) _logger.Log($"Symlinking {currentInputFile}");
                            File.CreateSymbolicLink(currentInputFile.ToDebugAssemblyPath(baseInputPath, DebugSettings.DebugOutputPath), currentInputFile);
                        }
                        else
                        {
                            if (_verbose) _logger.Log($"Copying {currentInputFile}");
                            File.Copy(currentInputFile, currentInputFile.ToDebugAssemblyPath(baseInputPath, DebugSettings.DebugOutputPath));
                        }

                        progressStruct.UnitsCompleted++;
                        _progress.Report(progressStruct);
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to copy file {ex.Message}!");
                }
            });

            timer.Stop();
            _logger.Log($"Copy took {timer.Elapsed.TotalSeconds} seconds...");
        }
    }
}
