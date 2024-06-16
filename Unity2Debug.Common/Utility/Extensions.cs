namespace Unity2Debug.Common.Utility
{
    public static class Extensions
    {
        public static string LongPath(this string path) => @"\\?\" + path;
        public static string TrimSeparator(this string s)
        {
            return s.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

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
    }
}
