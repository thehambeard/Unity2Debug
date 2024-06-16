using System.Diagnostics;

namespace Unity2Debug.Common
{
    public static class CommandRunner
    {
        public static int Run(string command, string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = $"\"{command}\"",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new() { StartInfo = processStartInfo })
            {
                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
