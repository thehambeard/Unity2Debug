using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Unity2Debug.Common.Utility
{
    public class FileMatcher
    {
        private readonly Dictionary<string, Matcher> _fileFilters;

        public FileMatcher(HashSet<string> filters)
        {
            _fileFilters = [];
            RegisterFilters(filters);
        }

        public bool Match(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            var directory = Path.GetDirectoryName(filePath);
            var file = Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(file))
                return false;

            if (_fileFilters.TryGetValue(directory, out Matcher? value))
                return value.Match(file).HasMatches;

            return false;
        }

        private void RegisterFilters(HashSet<string> filters)
        {
            foreach (var filter in filters)
            {
                var path = Path.GetDirectoryName(filter);
                var pattern = Path.GetFileName(filter);

                if (string.IsNullOrEmpty(path) || !Directory.Exists(path) || string.IsNullOrEmpty(pattern))
                    continue;

                if (_fileFilters.TryGetValue(path, out Matcher? value))
                    value.AddInclude(pattern);
                else
                {
                    Matcher matcher = new();
                    matcher.AddInclude(pattern);

                    _fileFilters.Add(path, matcher);
                }
            }
        }
    }
}
