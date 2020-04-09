namespace LionFire.ExtensionMethods
{
    public static class PathExtensions
    {
        public static bool PathEqualsOrIsDescendant(this string path, string directory) 
            => path == directory || path.StartsWith(directory + System.IO.Path.DirectorySeparatorChar);

    }
}
