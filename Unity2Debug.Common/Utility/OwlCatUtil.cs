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
        public static string FindWrathPath()
        {
            var path = FindInstallLocation("Pathfinder Wrath Of The Righteous", "Player.log", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/Wrath_Data/Managed'$");

            if(string.IsNullOrEmpty(path))
                path = FindInstallLocation("Pathfinder Wrath Of The Righteous", "Player.log", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/wrath_Data/Managed'$");

            return path;
        }
        public static string FindRTPath() => FindInstallLocation("Warhammer 40000 Rogue Trader", "Player.log", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/WH40KRT_Data/Managed'$");
        public static string FindKMPath() => FindInstallLocation("Pathfinder Kingmaker", "output_log.txt", "Mono path[0]", @"^Mono path\[0\] = '(.*?)/Kingmaker_Data/Managed'$");


        private static string FindInstallLocation(string gameFolderName, string logName, string line, string regex)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var rogueTraderDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low", "Owlcat Games", gameFolderName, logName);
                string lineToFind = "Mono path[0]";
                foreach (var lineIter in File.ReadLines(rogueTraderDataPath))
                {
                    if (lineIter.Contains(lineToFind))
                    {
                        line = lineIter;
                        break;
                    }
                }
                string monoPathRegex = regex;
                Match match = Regex.Match(line, monoPathRegex);
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
