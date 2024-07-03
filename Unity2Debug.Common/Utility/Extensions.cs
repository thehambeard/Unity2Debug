namespace Unity2Debug.Common.Utility
{
    public static class Extensions
    {
        public static string LongPath(this string path) => @"\\?\" + path;
        public static string TrimSeparator(this string s)
        {
            return s.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static string NormalizePath(this string path) => Path.Combine(path.Split('/', '\\'));

        public static string EnsureSeparator(this string s)
        {
            if (s.EndsWith(Path.DirectorySeparatorChar) || s.EndsWith(Path.AltDirectorySeparatorChar))
                return s;

            return s + Path.DirectorySeparatorChar;
        }

        public static T[] AddTo<T>(this T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;

            return array;
        }

        public static HashSet<string> GetFullSymlinkDirectories(this List<string> symlinks, string retailBasePath)
        {
            HashSet<string> result = [];

            foreach (var link in symlinks.Where(link => link.EndsWith('\\')))
                result.Add(Path.Combine(retailBasePath, link).TrimSeparator());

            return result;
        }

        public static HashSet<string> GetFullSymlinkFileFilters(this List<string> symlinks, string retailBasePath)
        {
            HashSet<string> result = [];

            foreach (var link in symlinks.Where(link => !link.EndsWith('\\')))
                result.Add(Path.Combine(retailBasePath, link));

            return result;
        }

        public static string ToDebugAssemblyPath(this string assemblyPath, string basePath, string debugOutputPath)
        {
            return assemblyPath.Replace(basePath, debugOutputPath);
        }

        public static List<string> ToDebugAssemblyPaths(this List<string> assemblyPaths, string basePath, string debugOutputPath)
        {
            if (assemblyPaths == null || assemblyPaths.Count == 0) return [];

            return assemblyPaths.Select(x => x.ToDebugAssemblyPath(basePath, debugOutputPath)).ToList();
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            foreach (var item in collection)
                hashSet.Add(item);
        }
    }
}
