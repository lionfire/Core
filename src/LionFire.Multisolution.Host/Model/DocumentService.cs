using Microsoft.Extensions.Options;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Text.Json;

namespace LionFire.MultiSolution.Host.Model;

public class DocumentsOptions
{
    public bool LoadMostRecent { get; set; }
    public List<string> MostRecent { get; set; }

}

public class DocumentService : ReactiveObject
{
    [Reactive]
    public MultiSolutionDocument Document { get; set; } = new();

    [Reactive]
    public string? DocumentPath { get; set; } 

    public IOptionsMonitor<DocumentsOptions> OptionsMonitor { get; }
    public DocumentsOptions Options => OptionsMonitor.CurrentValue

    public DocumentService(IOptionsMonitor<DocumentsOptions> optionsMonitor)
    {
        OptionsMonitor = optionsMonitor;
        TryLoadMostRecent();
    }

    public bool TryLoadMostRecent()
    {
        if (!Options.LoadMostRecent) return false;

        var path = Options?.MostRecent.FirstOrDefault();
        if (string.IsNullOrEmpty(path)) return false;
        return TryLoad(path);
    }

    public bool TryLoad(string path)
    {
        var json = File.ReadAllText(path);
        var doc = JsonSerializer.Deserialize<MultiSolutionDocument>(json);

        Document = doc;

        DocumentPath = doc == null ? null : path;
        return doc != null;
    }

    public void Save(string? path = null)
    {
        path ??= DocumentPath;
        if (path == null) throw new ArgumentNullException($"{nameof(Path)} or {nameof(DocumentPath)} must be set");

        var json = JsonSerializer.Serialize<MultiSolutionDocument>(Document);
        File.WriteAllText(path, json);
    }
}
