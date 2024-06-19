using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ILSpyAutomation
{
    public static class ILSpyAutomation
    {
        public static ProjectItem Decompile(string inputFile, string outputDirectory, IDecompileLogger logger, IProgress<DecompilationProgress> progress, bool generatePDB = true)
        {
            Directory.CreateDirectory(outputDirectory);

            string projectFileName = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + ".csproj");

            logger.Log($"Decompile Started {inputFile}");

            var pid = DecompileAsProject(inputFile, projectFileName, progress);

            if (generatePDB)
            {
                logger.Log($"Generating PDB");

                GeneratePdbForAssembly(inputFile, Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + ".pdb"), progress, outputDirectory);
            }

            return new ProjectItem(projectFileName, pid.PlatformName, pid.Guid, pid.TypeGuid);
        }

        private static ProjectId DecompileAsProject(string assemblyFileName, string projectFileName, IProgress<DecompilationProgress> progress)
        {
            var module = new PEFile(assemblyFileName);
            var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Metadata.DetectTargetFrameworkId());
            resolver.AddSearchDirectory(Path.GetFullPath(assemblyFileName));
            var decompiler = new WholeProjectDecompiler(GetSettings(module), resolver, null, resolver, null);
            decompiler.ProgressIndicator = progress;
            using (var projectFileStream = File.OpenWrite(projectFileName))
            using (var projectFileWriter = new StreamWriter(projectFileStream))
            {
                return Task.Run(() =>
                {
                    return decompiler.DecompileProject(module, Path.GetDirectoryName(projectFileName), projectFileWriter);
                }).Result;
            }
        }

        private static DecompilerSettings GetSettings(PEFile module)
        {
            return new DecompilerSettings()
            {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = false,
                RemoveDeadStores = false,
                UseSdkStyleProjectFormat = WholeProjectDecompiler.CanUseSdkStyleProjectFormat(module),
                UseNestedDirectoriesForNamespaces = true,
            };
        }

        private static int GeneratePdbForAssembly(string assemblyFileName, string pdbFileName, IProgress<DecompilationProgress> progress, string namePrefix)
        {
            var module = new PEFile(assemblyFileName,
                new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read),
                PEStreamOptions.PrefetchEntireImage,
                metadataOptions: MetadataReaderOptions.None);

            if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(module))
                return -1;

            using (FileStream stream = new FileStream(pdbFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var decompiler = GetDecompiler(assemblyFileName);
                var progess = progress;

                PortablePdbWriter.WritePdb(module, decompiler, GetSettings(module), stream, namePrefix: namePrefix, progress: progess, noLogo: true);
            }

            return 0;
        }

        private static CSharpDecompiler GetDecompiler(string assemblyFileName)
        {
            var module = new PEFile(assemblyFileName);
            var resolver = new UniversalAssemblyResolver(assemblyFileName, false, module.Metadata.DetectTargetFrameworkId());

            resolver.AddSearchDirectory(Path.GetFullPath(assemblyFileName));

            return new CSharpDecompiler(assemblyFileName, resolver, GetSettings(module));
        }
    }
}
