
namespace LionFire.UI;

public interface IObjectExplorerRoots
{
    IReadOnlyDictionary<string, object> Roots { get; }
}

public class ObjectExplorerRoots : IObjectExplorerRoots
{
    public IDictionary<string, object> Roots => roots;

    IReadOnlyDictionary<string, object> IObjectExplorerRoots.Roots => roots;

    protected Dictionary<string, object> roots = new();
}
