public static class UriUtils
{
    public static string GetFilenameFromUri(string uri) => Path.GetFileName(new Uri(uri).LocalPath);

}
