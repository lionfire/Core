using LionFire.Inspection;
using LionFire.Inspection.Nodes;
using LionFire.IO;
using LionFire.Structures;

public abstract class GroupInfo : IKeyed<string>, INodeInfo
{
    #region Identity

    public string Key { get; protected set; }

    #endregion

    #region Relationships

    public IInspector Inspector { get; set; }

    public string? Parent { get; set; }

    #region Derived: Depth

    /// <summary>
    /// How many parents this group has.  0 means it is a root group.  The higher the number, the more likely it is that the group is a more sophisticated and tailored way to inspect the object.
    /// </summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            var p = Parent;
            while (p != null)
            {
                depth++;
                p = Parent;
            }
            return depth;
        }
    }

    #endregion

    #endregion

    #region Lifecycle

    public GroupInfo(string key)
    {
        Key = key;
    }

    #endregion

    #region Properties

    public string? Order { get; set; }

    public Type? Type => throw new NotImplementedException();

    #region INodeInfo

    public string? Name { get; set; }
    public string DisplayName { 
        get => displayName ?? Name ?? (Type?.Name ?? "{Group}");
        set => displayName = value;
    }
    string? displayName;

    public InspectorNodeKind NodeKind => InspectorNodeKind.Group;

    public IEnumerable<string> Flags => Enumerable.Empty<string>();

    public IODirection IODirection => IODirection.Read; // FUTURE: ReadWrite for writable collections

    #endregion

    #endregion

    public abstract bool IsSourceTypeSupported(Type type);

    public abstract IInspectorGroup CreateFor(INode node);
}
