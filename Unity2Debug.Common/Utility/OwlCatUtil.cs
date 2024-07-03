using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Unity2Debug.Common.Utility
{
    internal class OwlCatUtil
    {
        public static string FindWrathPath() => FindInstallLocation("Pathfinder Wrath Of The Righteous", "Player.log", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/Wrath_Data/Managed'$").NormalizePath();
        public static string FindRTPath() => FindInstallLocation("Warhammer 40000 Rogue Trader", "Player.log", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/WH40KRT_Data/Managed'$").NormalizePath();
        public static string FindKMPath() => FindInstallLocation("Pathfinder Kingmaker", "output_log.txt", "[Manager] Mods path:", @"^\[Manager\] Mods path: (.*?)\\Mods.$").NormalizePath();

        private static string FindInstallLocation(string gameFolderName, string logName, string toFind, string regex)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Owlcat Games", gameFolderName, logName);
                    var line = string.Empty;

                    foreach (var lineIter in File.ReadLines(DataPath))
                    {
                        if (lineIter.Contains(toFind))
                        {
                            line = lineIter;
                            break;
                        }
                    }
                    Match match = Regex.Match(line, regex);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch
            {
                return "";
            }

            return "";
        }
    }
}
