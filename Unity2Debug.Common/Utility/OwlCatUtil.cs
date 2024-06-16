using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Unity2Debug.Common.Utility
{
    internal class OwlCatUtil
    {
        public static string FindWrathPath() => FindInstallLocation("Pathfinder Wrath Of The Righteous", "Player.log", @"^Mono path\[0\] = '(.*?)/Wrath_Data/Managed'$");
        public static string FindRTPath() => FindInstallLocation("Warhammer 40000 Rogue Trader", "Player.log", @"^Mono path\[0\] = '(.*?)/WH40KRT_Data/Managed'$");
        public static string FindKMPath() => FindInstallLocation("Pathfinder Kingmaker", "output_log.txt", @"^Mono path\[0\] = '(.*?)/Kingmaker_Data/Managed'$");

        private static string FindInstallLocation(string gameFolderName, string logName, string regex)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var rogueTraderDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Owlcat Games", gameFolderName, logName);
                string lineToFind = "Mono path[0]";
                var line = string.Empty;
                foreach (var lineIter in File.ReadLines(rogueTraderDataPath))
                {
                    if (lineIter.Contains(lineToFind))
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
            else
            {
            }
            return "";
        }
    }
}
