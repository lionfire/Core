public class ScriptList
{
    public string[] Uris { get; set; }

    public ScriptList(params string[] urls)
    {
        Uris = urls;
    }

    public IEnumerable<string> Files => Uris.Select(u => Path.Combine(AppConfig.DownloadDir, UriUtils.GetFilenameFromUri(u)));

}
