
namespace LionFire.Data.Async.Gets;

// TODO: implement this
// TODO: replicate this for Setter and Value
// TODO: replicate this for Async Collections
public interface IConfiguredGetter : IGetter
{
    GetterOptions GetterOptions { get; }
    GetterFeatures Features => GetterFeatures.Unspecified;
}

public interface IConfigurableGetter : IConfiguredGetter
{
    new GetterOptions GetterOptions { get; set; }
}