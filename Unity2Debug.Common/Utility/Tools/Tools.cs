using Mono.Cecil;
using Mono.Cecil.Cil;


namespace Unity2Debug.Common.Utility.Tools
{
    public static class Tools
    {
        private const string _de4dot = "de4dot\\de4dot-x64.exe";
        private const string _peupdate = "peupdate\\peupdate.exe";

        public static bool HasDebugDirectory(string assemblyPath)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(assemblyPath))
            {
                foreach (var entry in assembly.MainModule.GetDebugHeader().Entries)
                {
                    if (entry.Directory.Type == ImageDebugType.CodeView)
                        return true;
                }
            }

            return false;
        }

        public static int De4dot(string input, string output)
        {
            var de4dot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _de4dot);

            if (!File.Exists(de4dot))
                throw new FileNotFoundException(de4dot);

            return CommandRunner.Run(de4dot, $"--dont-rename --keep-types --preserve-tokens --preserve-strings -fpdb \"{input}\" -o \"{output}\"");
        }

        public static int PeUpdate(string input, string output)
        {
            var peUpdate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _peupdate);

            if (!File.Exists(peUpdate))
                throw new FileNotFoundException(peUpdate);

            return CommandRunner.Run(peUpdate, $"-u \"{output}\" \"{input}\"");
        }
    }
}
