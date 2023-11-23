using LionFire.Inspection.ViewModels;

namespace LionFire.Inspection.Nodes;

public class NodeVMComparer : IComparer<NodeVM>
{
    //NodeInfoSorter
    //NodeInfoKVSorter
    public static readonly NodeVMComparer Instance = new();

    public int Compare(NodeVM? x, NodeVM? y)
        => NodeInfoComparer.Instance.Compare(x?.Node.Info, y?.Node.Info);
}
