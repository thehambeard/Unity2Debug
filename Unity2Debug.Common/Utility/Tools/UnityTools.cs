using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Unity2Debug.Common.Utility.Tools
{
    public partial class UnityTools
    {
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

        public static string GetUnityVersionFromAssembly(string gameExePath)
        {
            if (!File.Exists(gameExePath))
                return string.Empty;

            var fileInfo = FileVersionInfo.GetVersionInfo(gameExePath);

            if (fileInfo.ProductVersion != null)
            {
                var match = UnityVersionRegex().Match(fileInfo.ProductVersion);

                if (match.Success)
                    return match.Value;
            }

            return string.Empty;
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

            var appFolder = fullPath.Substring(0, index + appFolderName.Length);

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

            using (StreamReader reader = new StreamReader(filePath))
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
