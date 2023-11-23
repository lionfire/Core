namespace LionFire.Inspection.Nodes;

public class NodeInfoComparer : IComparer<INodeInfo>
{
    public static readonly NodeInfoComparer Instance = new();

    public int Compare(INodeInfo? x, INodeInfo? y)
    {
        if (x?.Order.HasValue == true)
        {
            if (y.Order.HasValue == true)
            {
                if (x.Order.Value == y.Order.Value)
                {
                    return String.Compare(x.Name, y.Name);
                }
                else
                {
                    return x.Order.Value.CompareTo(y.Order.Value);
                }
            }
            else
            {
                return x.Order.Value >= 0 ? 1 : -1;
            }
        }
        else
        {
            if (y.Order.HasValue == true)
            {
                return y.Order.Value >= 0 ? -1 : 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
