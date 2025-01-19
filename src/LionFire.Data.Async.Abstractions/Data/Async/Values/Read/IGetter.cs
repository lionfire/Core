
namespace LionFire.Data.Async.Gets;

/// <summary>
/// Marker interface indicating that at least one IStatelessGetter&lt;TValue&gt; should also present.
/// </summary>
public interface IGetter { }  // RENAME IGettable.  Make IGetter a service that gets things (not itself)

