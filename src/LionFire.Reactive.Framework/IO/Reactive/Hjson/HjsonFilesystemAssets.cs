#if UNUSED
using DynamicData;
using Hjson;
using System.Reactive.Linq;

namespace LionFire.Trading.Link.Blazor.Components.Pages;

public abstract class ObservableAssetBase<TValue>
{
}
public class FilesystemAssetOptions<TValue>
{
    public string? Dir { get; set; } = @$"z:\assets\{typeof(TValue).Name}"; // TEMP default
}
public class HjsonFilesystemAssets<TValue> : ObservableAssetBase<TValue>, IObservableReader<TValue>
{
    #region Parameters

    private string dir;

    #endregion

    #region Lifecycle

    public HjsonFilesystemAssets(IOptionsSnapshot<FilesystemAssetOptions<TValue>> optionsSnapshot)
    {
        dir = optionsSnapshot.Value.Dir;
    }

    #endregion

    #region State

    private SourceList<string> keys = new();

    private SourceList<string> knownInvalidKeys = new();

    #endregion

    #region IObservableAssets

    public IObservableList<string> Keys => keys;

    public IObservable<TValue?> Listen(string key)
    {

    }

    #endregion

    protected override TValue Deserialize(byte[] underlying)
    {
        var hjson = UTF8Encoding.UTF8.GetString(underlying);
        var json = HjsonValue.Parse(hjson).ToString(Stringify.Plain);
        return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }

    protected override byte[] Serialize(TValue usable)
    {
        var json = JsonSerializer.Serialize<TValue>(usable);
        var hjson = JsonValue.Parse(json).ToString(new HjsonOptions { EmitRootBraces = false });
        return UTF8Encoding.UTF8.GetBytes(json);
    }
}

#endif