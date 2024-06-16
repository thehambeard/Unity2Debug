using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using System.Diagnostics;
using Unity2Debug.Common.Logging;
using Unity2Debug.Common.SettingsService;
using Unity2Debug.Common.Utility;

namespace Unity2Debug.Common.Automation
{
    public class CopyGameAutomator
    {
        public CancellationToken CancellationToken { get; set; }
        public DebugSettings DebugSettings { get; set; }
        public DecompileSettings DecompileSettings { get; set; }

        private readonly ILogger _logger;

        public CopyGameAutomator(DebugSettings debugSettings, DecompileSettings decompileSettings, ILogger logger, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            DebugSettings = debugSettings;
            DecompileSettings = decompileSettings;
            _logger = logger;
        }

        public async Task CopyGameAsync()
        {
            var timer = new Stopwatch();
            timer.Start();

            await Task.Run(() =>
            {
                try
                {
                    _logger.Log("Creating Debug Directories.");

                    var baseInputPath = Path.GetDirectoryName(DebugSettings.RetailGameExe)
                        ?? throw new NullReferenceException();

                    List<string> symlinkPaths = [];

                    if (DebugSettings.UseSymlinks)
                        symlinkPaths = DebugSettings.GetFullSymlinkDirectories();

                    foreach (string currentInputPath in Directory.GetDirectories(baseInputPath, "*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            var currentOutputPath = currentInputPath.Replace(baseInputPath, DebugSettings.DebugOutputPath);

                            CancellationToken.ThrowIfCancellationRequested();

                            if (DebugSettings.ExcludeDirectories.Contains(currentInputPath) || DebugSettings.ExcludeDirectories.Any(excludePath => GetParentDirectories(currentInputPath).Contains(excludePath)))
                                continue;

                            if (!Directory.Exists(currentOutputPath))
                            {
                                if (DebugSettings.UseSymlinks)
                                {
                                    if (symlinkPaths.Contains(currentInputPath))
                                    {
                                        _logger.Log($"Symlinking: {currentInputPath}");
                                        Directory.CreateSymbolicLink(currentOutputPath, currentInputPath);
                                        continue;
                                    }

                                    if (symlinkPaths.Any(symlinkPath => GetParentDirectories(currentInputPath).Contains(symlinkPath)))
                                        continue;
                                }
                                _logger.Log($"Creating Directory: {currentOutputPath}");
                                Directory.CreateDirectory(currentOutputPath.LongPath());
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Failed to copy directory {currentInputPath}!");
                            _logger.Error(ex);
                        }
                    }

                    _logger.Log("Copying Files.");

                    FileMatcher fileMatcher = new(DebugSettings.GetFullSymlinkFileFilters());

                    if (DebugSettings.UseSymlinks)
                        DebugSettings.ExcludeDirectories.AddRange(symlinkPaths);
                    
                    foreach (string currentInputFile in Directory.GetFiles(baseInputPath, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            CancellationToken.ThrowIfCancellationRequested();

                            //if (DecompileSettings.AssemblyPaths.Contains(currentInputFile))
                            //    continue;

                            if (DebugSettings.ExcludeDirectories.Any(excludePath => GetParentDirectories(currentInputFile).Contains(excludePath)))
                                continue;

                            var currentOutputFile = currentInputFile.Replace(baseInputPath, DebugSettings.DebugOutputPath);

                            if (DebugSettings.UseSymlinks && fileMatcher.Match(currentInputFile))
                            {
                                _logger.Log($"Symlinking: {currentInputFile}");
                                File.CreateSymbolicLink(currentOutputFile.LongPath(), currentInputFile.LongPath());
                            }
                            else
                            {
                                _logger.Log($"Copying: {currentInputFile}");
                                File.Copy(currentInputFile.LongPath(), currentOutputFile.LongPath(), true);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Failed to copy file {currentInputFile}!");
                            _logger.Error(ex);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }

            }, CancellationToken);

            timer.Stop();
            _logger.Log($"Copy took {timer.Elapsed.TotalSeconds} seconds...");
        }

        private static IEnumerable<string> GetParentDirectories(string path)
        {
            var parent = Path.GetDirectoryName(path);

            if (parent is null) yield break;

            yield return parent;

            foreach (var ancestor in GetParentDirectories(parent))
                yield return ancestor;
        }
    }
}
