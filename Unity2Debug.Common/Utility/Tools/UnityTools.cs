using ICSharpCode.Decompiler.CSharp.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity2Debug.Common.SettingsService;

namespace Unity2Debug.Common.Utility.Tools
{
    public partial class UnityTools
    {
        public static string? GetUnityMonoPath(string unityInstallPath)
        {
            var path = Path.GetDirectoryName(unityInstallPath);

            if (path == null)
                return null;

            string? result = Path.Combine(path, UnityConstants.DEV64_MONO_PATH);

            if (!Directory.Exists(result))
                result = Path.Combine(path, UnityConstants.DEV64_MONO_PATH2);

            return Directory.Exists(result) ? result : null;
        }

        public static bool IsAssemblyVersionMatch(out string unityVersion, string unityInstallPath, string gameExePath)
        {
            var gameVersion = GetUnityVersionFromAssembly(gameExePath);
            unityVersion = GetUnityVersionFromAssembly(unityInstallPath) ?? string.Empty;
            return !string.IsNullOrEmpty(unityVersion) && gameVersion == unityVersion;
        }

        public static bool TryGetVaildUnityPath(out (string path, string version)? validPathAndVersion, string basePath, string gameExePath)
        {
            validPathAndVersion = null;
            string? dir;
            string?[] possiblePaths = 
            [
                Path.Combine(basePath, "Unity.exe"),
                Path.Combine(basePath, UnityConstants.UNITY_EXE_PATH),
                GetUnityExeFromHubdirectory(basePath, gameExePath)
            ];

            foreach (var testPath in possiblePaths)
            {
                if (testPath != null && File.Exists(testPath) && IsAssemblyVersionMatch(out var version, testPath, gameExePath) && (dir = Path.GetDirectoryName(testPath)) != null)
                {
                    validPathAndVersion = (dir, version);
                    return true;
                }
            }

            return false;
        }

        public static string? GetUnityExeFromHubdirectory(string hubDirectory, string gameExePath)
        {
            var version = GetUnityVersionFromAssembly(gameExePath);

            if (string.IsNullOrEmpty(version))
                return null;

            if (GetUnityVersionsInPath(hubDirectory).Contains($"{version}f1"))
                version = $"{version}f1";

            var path = Path.Combine(hubDirectory, version, UnityConstants.UNITY_EXE_PATH);

            return File.Exists(path) ? path : null;
        }

        public static List<string> GetUnityVersionsInPath(string path)
        {
            List<string> result = [];

            if (!Directory.Exists(path))
                return result;

            foreach (var dir in Directory.GetDirectories(path))
            {
                var name = Path.GetFileName(dir).TrimSeparator();
                if (name != null && UnityVersionRegex().IsMatch(name))
                    result.Add(name);
            }

            return result;
        }

        public static string? GetUnityVersionFromAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                return null;

            var fileInfo = FileVersionInfo.GetVersionInfo(assemblyPath);

            if (fileInfo.ProductVersion != null)
            {
                var match = UnityVersionRegex().Match(fileInfo.ProductVersion);

                if (match.Success)
                    return $"{match.Value}f1";
            }

            return null;
        }

        public static string? GetSteamAppId(string gamePath)
        {
            string? appId = "";
            var appFolderName = "steamapps";
            var fullPath = Path.GetDirectoryName(gamePath);
            var installDir = Path.GetFileName(fullPath);

            if (!Directory.Exists(fullPath) || installDir == null)
                return string.Empty;

            int index = fullPath.LastIndexOf(appFolderName);

            if (index == -1)
                return string.Empty;

            var appFolder = fullPath[..(index + appFolderName.Length)];

            if (appFolder == null && !Directory.Exists(appFolder))
                return string.Empty;

            foreach (var file in Directory.GetFiles(appFolder))
                if ((appId = CheckACFforAppID(file, installDir)) != null)
                    break;

            return appId;
        }

        private static string? CheckACFforAppID(string filePath, string installDir)
        {
            string? appId = null;

            using (StreamReader reader = new(filePath))
            {
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("\"appid\""))
                    {
                        var match = DigitRegex().Match(line);

                        if (match.Success)
                            appId = match.Value;

                        continue;
                    }

                    if (line.Contains("\"installdir\""))
                    {
                        if (!line.Contains(installDir))
                            appId = null;

                        break;
                    }
                }
            }
            return appId;
        }

        [GeneratedRegex(@"^\d{4}\.\d\.\d{1,2}")]
        public static partial Regex UnityVersionRegex();

        [GeneratedRegex(@"\d+")]
        public static partial Regex DigitRegex();
    }
}
