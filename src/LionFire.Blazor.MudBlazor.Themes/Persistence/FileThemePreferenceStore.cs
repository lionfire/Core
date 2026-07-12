namespace LionFire.Blazor.MudBlazor_.Theming.Persistence;

/// <summary>
/// Default store: one small text file per preference under a directory of the host's
/// choosing (defaults to <c>~/.config/lionfire/appearance</c>). Suitable for a
/// single-operator app; multi-user hosts should supply their own store.
///
/// Every operation swallows IO failures — an unwritable config directory downgrades
/// appearance to "not persisted", it does not take down the shell.
/// </summary>
public sealed class FileThemePreferenceStore : IThemePreferenceStore
{
    private readonly string _directory;

    public FileThemePreferenceStore(string? directory = null)
    {
        _directory = directory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config", "lionfire", "appearance");
    }

    private string PathFor(string key) => Path.Combine(_directory, key.Replace('.', '-'));

    public async ValueTask<string?> GetAsync(string key)
    {
        try
        {
            var path = PathFor(key);
            if (!File.Exists(path)) return null;
            return (await File.ReadAllTextAsync(path)).Trim();
        }
        catch
        {
            return null;
        }
    }

    public async ValueTask SetAsync(string key, string? value)
    {
        try
        {
            var path = PathFor(key);
            if (value is null)
            {
                if (File.Exists(path)) File.Delete(path);
                return;
            }

            Directory.CreateDirectory(_directory);
            await File.WriteAllTextAsync(path, value);
        }
        catch
        {
            // Ignore — the UI still reflects the in-memory change.
        }
    }
}

/// <summary>A store that persists nothing. Preferences last for the life of the circuit.</summary>
public sealed class NullThemePreferenceStore : IThemePreferenceStore
{
    public ValueTask<string?> GetAsync(string key) => ValueTask.FromResult<string?>(null);
    public ValueTask SetAsync(string key, string? value) => ValueTask.CompletedTask;
}
