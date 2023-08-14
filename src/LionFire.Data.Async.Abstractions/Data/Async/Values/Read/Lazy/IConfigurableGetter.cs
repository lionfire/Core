
namespace LionFire.Data.Async.Gets;

// TODO: implement this
// TODO: replicate this for Setter and Value
// TODO: replicate this for Async Collections
public interface IConfigurableGetter
{
    GetterOptions Options { get; set; }
    GetterFeatures Features { get; }
}
