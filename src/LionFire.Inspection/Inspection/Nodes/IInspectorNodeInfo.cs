namespace LionFire.Inspection.Nodes;

public interface INodeInfo
{

    string? Name { get; }

    /// <summary>
    /// If Order is null, Name should be considered instead.
    /// 
    /// If it is a number, it will be cast to a float. Nummbers will appear before alphabetical orders.
    /// Negative numbers will appear at the end.
    /// 
    /// Example order:
    /// 
    /// 0
    /// 1
    /// 2
    /// 00000003
    /// Axx
    /// axx
    /// Bxx
    /// bxx
    /// -3
    /// -0000002.1
    /// -0002.05
    /// -1
    /// </summary>
    string? Order { get; set; }

    InspectorNodeKind NodeKind { get; }

    IEnumerable<string> Flags { get; }
}
